import { AnyObject, PlatePlugin } from '@udecode/plate';

import { AppFile } from '@/types/file';
import { CustomEditor, CustomValue } from '@/types/plateEditor';

export interface CommentContentProps {
	value: CustomValue;
	files: AppFile[];
	plugins: PlatePlugin<AnyObject, CustomValue, CustomEditor>[];
	editorId: string;
}
