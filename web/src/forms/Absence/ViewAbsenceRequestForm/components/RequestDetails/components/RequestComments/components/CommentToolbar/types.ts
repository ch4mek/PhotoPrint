import { PlateEditor } from '@udecode/plate';
import { Dispatch } from 'react';

import { AppFile } from '@/types/file';
import { CustomValue } from '@/types/plateEditor';

export interface CommentToolbarProps {
	files: AppFile[];
	editor: PlateEditor<CustomValue>;
	onSetFiles: Dispatch<React.SetStateAction<AppFile[]>>;
}
