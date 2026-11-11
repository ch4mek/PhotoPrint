interface User {
	userId: number;
	firstName: string;
	lastName: string;
	email: string;
	departmentId: string;
}

interface ArticleReference {
	id: number;
	internalFileId: string;
	externalUrl: string;
}

export interface FullArticleItem {
	id: number;
	groupId: number;
	name: string;
	icon: string;
	views: number;
	text: string;
	keywords: string;
	isActive: boolean;
	isAvailableOnMainScreen: boolean;
	orderWeight: number;
	createdByUser: User;
	createdAtUtc: string;
	updatedByUser: User;
	updatedAtUtc: string;
	articleReferences: ArticleReference[];
	tags: string[];
}

export interface KnowledgeGroupItem {
	id: number;
	parentGroupId: number | null;
	name: string;
	icon: string | null;
	isActive: boolean;
	isAvailableOnMainScreen: boolean;
	orderWeight: number;
	createdByUser: User;
	createdAtUtc: string;
	updatedByUser: User;
	updatedAtUtc: string | null;
	articles: ArticleItem[];
	childGroups: KnowledgeGroupItem[];
}

interface ArticleItem {
	id: number;
	name: string;
	icon: null | string;
	isActive: boolean;
	isAvailableOnMainScreen: boolean;
	orderWeight: number;
	createdAtUtc: string;
	updatedAtUtc: null | string;
}

export interface AddGroupParams {
	parentGroupId: number | null;
	name: string;
	icon: string | null;
	isActive: boolean;
	isAvailableOnMainScreen: boolean;
}

export interface UpdateGroupNameParam {
	id: number;
	name: string;
}

interface ArticleParams {
	groupId: number | null;
	name: string;
	icon: string;
	text: string;
	keywords: string;
	isActive: boolean;
	isAvailableOnMainScreen: boolean;
	orderWeight?: number;
	internalFieldIds?: string[];
	externalUrls?: string[];
}

export interface AddArticleParams extends ArticleParams {
	tags: string[];
}

export interface UpdateArticleParams extends ArticleParams {
	id: number | string;
	articleTags: string[];
	articleReferences?: ArticleReference[];
}

export interface SearchArticleParams {
	groupId?: number;
	tagId?: number;
	name?: string;
	keywords?: string;
}

export type SearchArticleResponse = FullArticleItem[];

export interface ArticleOrder {
	id: number;
	orderWeight: number;
	isGroup?: boolean;
}

export interface GroupOrder {
	id: number;
	orderWeight: number;
	parentGroupId: number | null;
	articleOrders: ArticleOrder[];
	childGroups: GroupOrder[];
}

export interface GroupOrderItem {
	groupOrders: GroupOrder[];
}
