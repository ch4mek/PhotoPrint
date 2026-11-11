import { AddArticleParams, AddGroupParams } from './types';

export const DEFAULT_ADD_GROUP_PARAMS: AddGroupParams = {
	parentGroupId: null,
	name: '',
	icon: 'no-icon',
	isActive: true,
	isAvailableOnMainScreen: true,
};

export const DEFAULT_ADD_ARTICLE_PARAMS: AddArticleParams = {
	groupId: null,
	name: '',
	icon: 'no-icon',
	keywords: '',
	isActive: true,
	isAvailableOnMainScreen: true,
	orderWeight: 0,
	internalFieldIds: [],
	externalUrls: [],
	tags: [],
	text: '',
};
