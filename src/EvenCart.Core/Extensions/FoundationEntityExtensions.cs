﻿#region License
// Copyright (c) Sojatia Infocrafts Private Limited.
// The following code is part of EvenCart eCommerce Software (https://evencart.co) Dual Licensed under the terms of
// 
// 1. GNU GPLv3 with additional terms (available to read at https://evencart.co/license)
// 2. EvenCart Proprietary License (available to read at https://evencart.co/license/commercial-license).
// 
// You can select one of the above two licenses according to your requirements. The usage of this code is
// subject to the terms of the license chosen by you.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EvenCart.Core.Data;
using EvenCart.Core.Services;

namespace EvenCart.Core.Extensions
{
    public static class FoundationEntityExtensions
    {
        public static void SetMeta(this FoundationEntity entity, string key, object data)
        {
            entity.MetaData = entity.MetaData ?? new Dictionary<string, object>();
            if (entity.MetaData.ContainsKey(key))
                entity.MetaData[key] = data;
            else
            {
                entity.MetaData.Add(key, data);
            }
        }

        public static T GetMeta<T>(this FoundationEntity entity, string key)
        {
            if (entity == null || entity.MetaData == null || !entity.MetaData.ContainsKey(key))
                return default(T);
            var value = entity.MetaData[key];
            if (value.GetType() == typeof(T))
                return (T)entity.MetaData[key];
            return default(T);
        }

        public static T GetWithTree<T>(this IFoundationEntityService<T> service, int id) where T : FoundationEntity, IAllowsParent<T>
        {
            var entity = service.Get(id);
            if (entity == null)
                return null;
            var currentEntity = entity;
            while (currentEntity.ParentId != 0)
            {
                currentEntity.Parent = service.Get(currentEntity.ParentId);
                currentEntity = currentEntity.Parent;
            }
            return entity;
        }

        public static IList<T> GetWithParentTree<T>(this IList<T> entities) where T : FoundationEntity, IAllowsParent<T>
        {
            foreach (var entity in entities)
            {
                MakeTree(entity, entities);
            }
            return entities.ToList();
        }

        public static string GetNameBreadCrumb<T>(this T entity, string separator = " > ") where T : FoundationEntity, IAllowsParent<T>, ISeoEntity
        {
            var builder = new StringBuilder();
            BuildBreadCrumb(entity, separator, builder);
            return builder.ToString().TrimEnd(separator.ToCharArray());
        }

        public static string GetFieldBreadCrumb<T>(this T entity, Func<T, string> fieldFunc, string separator = " > ") where T : FoundationEntity, IAllowsParent<T>
        {
            var builder = new StringBuilder();
            BuildFieldBreadCrumb(entity, fieldFunc, separator, builder);
            return builder.ToString().TrimEnd(separator.ToCharArray());
        }

        private static void MakeTree<T>(T parent, IList<T> allEntities) where T : FoundationEntity, IAllowsParent<T>
        {
            parent.Children = allEntities.Where(x => x.ParentId == parent.Id).ToList();
            foreach (var c in parent.Children)
                c.Parent = parent;
        }

        private static void BuildBreadCrumb<T>(T entity, string separator, StringBuilder builder) where T : FoundationEntity, IAllowsParent<T>, ISeoEntity
        {
            if (entity == null)
                return;
            BuildBreadCrumb(entity.Parent, separator, builder);
            if (entity.Parent != null)
                builder.Append(separator);
            builder.Append(entity.Name);
        }

        private static void BuildFieldBreadCrumb<T>(T entity, Func<T, string> fieldFunc, string separator, StringBuilder builder) where T : FoundationEntity, IAllowsParent<T>
        {
            if (entity == null)
                return;
            BuildFieldBreadCrumb(entity.Parent, fieldFunc, separator, builder);
            if (entity.Parent != null)
                builder.Append(separator);
            
            builder.Append(fieldFunc(entity));
        }
    }
}