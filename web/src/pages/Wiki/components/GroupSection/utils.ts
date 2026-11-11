import { GroupsItems } from '@/pages/Wiki/components/GroupSection/types';
import { Option, OptionWithoutPath } from '@/pages/Wiki/types';
import {
	ArticleOrder,
	FullArticleItem,
	GroupOrder,
	KnowledgeGroupItem,
} from '@/queries/wiki/types';
import { SegmentTypes } from '@/types/wiki';

const { Article, Group } = SegmentTypes;

const buildStructure = (data?: KnowledgeGroupItem[]): OptionWithoutPath[] => {
	if (!data || data.length === 0) {
		return [];
	}

	return data
		.sort(
			(firstGroup, secondGroup) =>
				firstGroup.orderWeight - secondGroup.orderWeight
		)
		.map((group) => {
			const childGroups = group.childGroups
				? buildStructure(group.childGroups)
				: [];
			const articles =
				group.articles?.map((article) => ({
					id: `${article.id}_${Article}`,
					isGroup: false,
					name: article.name,
					orderWeight: article.orderWeight,
					parentGroupId: `${group.id}_${Group}`,
				})) || [];

			return {
				id: `${group.id}_${Group}`,
				isGroup: true,
				expanded: false,
				name: group.name,
				parentGroupId:
					group.parentGroupId == null
						? group.parentGroupId
						: `${group.parentGroupId}_${Group}`,
				orderWeight: group.orderWeight,
				items: [...childGroups, ...articles].sort(
					(firstItem, secondItem) =>
						firstItem.orderWeight - secondItem.orderWeight
				),
			};
		});
};

export const assignAccessPaths = (
	items?: OptionWithoutPath[],
	parentPath = ''
): Option[] => {
	if (!items) {
		return [];
	}

	return items.map((item, index) => {
		const currentPath = `${parentPath}.items[${index}]`;

		return {
			...item,
			accessPath: currentPath,
			...(item.isGroup
				? { items: assignAccessPaths(item?.items, currentPath) }
				: {}),
		} as Option;
	});
};

export const serializeOptions = (data?: KnowledgeGroupItem[]): GroupsItems => ({
	items: assignAccessPaths(buildStructure(data)),
});

export const getIdFromUniqueGroupsIds = (id?: string | number): number => {
	if (typeof id === 'number') {
		return id;
	}

	if (!id) {
		return 0;
	}

	return Number(id.split('_')[0]);
};

export const transformOrderStructure = (
	items: Option[],
	parentGroupId: number | string | null = null
): GroupOrder[] =>
	items
		.map((item, index) => {
			const orderWeight = index;

			if (item.isGroup) {
				const childGroups: GroupOrder[] = [];
				const articleOrders: ArticleOrder[] = [];

				item.items?.forEach((subItem, subIndex) => {
					if (subItem.isGroup) {
						childGroups.push({
							id: getIdFromUniqueGroupsIds(subItem.id),
							orderWeight: subIndex,
							parentGroupId: getIdFromUniqueGroupsIds(item.id),
							articleOrders: (subItem.items ?? [])
								.map((article, articleIndex) => ({
									id: getIdFromUniqueGroupsIds(article.id),
									orderWeight: articleIndex,
									isGroup: article.isGroup,
								}))
								?.filter((i) => !i.isGroup) as ArticleOrder[],
							childGroups: transformOrderStructure(
								subItem.items || [],
								getIdFromUniqueGroupsIds(subItem.id)
							),
						});
					} else {
						articleOrders.push({
							id: getIdFromUniqueGroupsIds(subItem.id),
							orderWeight: subIndex,
						});
					}
				});

				return {
					id: getIdFromUniqueGroupsIds(item.id),
					orderWeight,
					parentGroupId,
					articleOrders,
					childGroups,
				} as GroupOrder;
			}

			return null;
		})
		.filter((item): item is GroupOrder => item !== null);

export const convertArticlesToOptions = (items: FullArticleItem[]) =>
	items.map<Option>(({ orderWeight, name, id, groupId }) => ({
		id,
		isGroup: false,
		name,
		orderWeight,
		parentGroupId: groupId,
		accessPath: '',
	}));
