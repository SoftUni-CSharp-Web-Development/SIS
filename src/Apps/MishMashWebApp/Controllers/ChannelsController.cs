﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MishMashWebApp.Models;
using MishMashWebApp.ViewModels.Channels;
using SIS.HTTP.Responses;
using SIS.MvcFramework;

namespace MishMashWebApp.Controllers
{
    public class ChannelsController : BaseController
    {
        [Authorize]
        public IHttpResponse Details(int id)
        {
            var channelViewModel = this.Db.Channels.Where(x => x.Id == id)
                .Select(x => new ChannelViewModel
                {
                    Type = x.Type,
                    Name = x.Name,
                    Tags = x.Tags.Select(t => t.Tag.Name),
                    Description = x.Description,
                    FollowersCount = x.Followers.Count(),
                }).FirstOrDefault();

            return this.View(channelViewModel);
        }

        [Authorize]
        public IHttpResponse Followed()
        {
            var followedChannels = this.Db.Channels.Where(
                    x => x.Followers.Any(f => f.User.Username == this.User))
                            .Select(x => new BaseChannelViewModel
                            {
                                Id = x.Id,
                                Type = x.Type,
                                Name = x.Name,
                                FollowersCount = x.Followers.Count(),
                            }).ToList();

            var viewModel = new FollowedChannelsViewModel
                {FollowedChannels = followedChannels};

            return this.View(viewModel);
        }

        [Authorize]
        public IHttpResponse Follow(int id)
        {
            var user = this.Db.Users.FirstOrDefault(x => x.Username == this.User);
            if (!this.Db.UserInChannel.Any(
                x => x.UserId == user.Id && x.ChannelId == id))
            {
                this.Db.UserInChannel.Add(new UserInChannel
                {
                    ChannelId = id,
                    UserId = user.Id,
                });

                this.Db.SaveChanges();
            }

            return this.Redirect("/Channels/Followed");
        }

        [Authorize]
        public IHttpResponse Unfollow(int id)
        {
            var user = this.Db.Users.FirstOrDefault(x => x.Username == this.User);
            
            var userInChannel = this.Db.UserInChannel.FirstOrDefault(
                x => x.UserId == user.Id && x.ChannelId == id);
            if (userInChannel != null)
            {
                this.Db.UserInChannel.Remove(userInChannel);
                this.Db.SaveChanges();
            }

            return this.Redirect("/Channels/Followed");
        }

        [Authorize]
        public IHttpResponse Create()
        {
            var user = this.Db.Users.FirstOrDefault(x => x.Username == this.User);
            if (user.Role != Role.Admin)
            {
                return this.Redirect("/Users/Login");
            }

            return this.View();
        }

        [Authorize]
        [HttpPost]
        public IHttpResponse Create(CreateChannelsInputModel model)
        {
            var user = this.Db.Users.FirstOrDefault(x => x.Username == this.User);
            if (user.Role != Role.Admin)
            {
                return this.Redirect("/Users/Login");
            }

            if (!Enum.TryParse(model.Type, true, out ChannelType type))
            {
                return this.BadRequestError("Invalid channel type.");
            }

            var channel = new Channel
            {
                Name = model.Name,
                Description = model.Description,
                Type = type,
            };

            if (!string.IsNullOrWhiteSpace(model.Tags))
            {
                var tags = model.Tags.Split(',', ';', StringSplitOptions.RemoveEmptyEntries);
                foreach (var tag in tags)
                {
                    var dbTag = this.Db.Tags.FirstOrDefault(x => x.Name == tag.Trim());
                    if (dbTag == null)
                    {
                        dbTag = new Tag {Name = tag.Trim()};
                        this.Db.Tags.Add(dbTag);
                        this.Db.SaveChanges();
                    }

                    channel.Tags.Add(new ChannelTag
                    {
                        TagId = dbTag.Id,
                    });
                }
            }

            this.Db.Channels.Add(channel);
            this.Db.SaveChanges();

            return this.Redirect("/Channels/Details?id=" + channel.Id);
        }
    }
}
