import { FullArticleItem } from '@/queries/wiki/types';
import { CustomRootBlock } from '@/types/plateEditor';

export interface PlateEditorProps {
	currentWikiPage: CustomRootBlock[];
	currentArticleData: FullArticleItem;
	isEmptyArticle: boolean;
	refetch: VoidFunction;
}
