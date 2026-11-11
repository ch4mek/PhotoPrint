import { FullArticleItem } from '@/queries/wiki/types';

export interface ArticleFormProps {
	articleId?: number | string;
	handleClose: VoidFunction;
	handleSubmit: (
		values: FullArticleItem | ArticleFormValues
	) => Promise<object | undefined> | void;
}

export interface ArticleFormValues {
	name: string;
	keywords: string;
}
