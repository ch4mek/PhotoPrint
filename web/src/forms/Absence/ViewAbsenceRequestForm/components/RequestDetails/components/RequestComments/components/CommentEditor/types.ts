import { ReactNode } from 'react';

import { AppFile } from '@/types/file';
import { CustomValue } from '@/types/plateEditor';

import { PlateModes } from './constants';

export interface CommentEditorProps {
	onSubmit: (commentText: string, files: AppFile[]) => Promise<void>;
	children: (props: { isSendDisabled: boolean }) => ReactNode;
	editorId: string;
	initialValue?: CustomValue;
	initialFiles?: AppFile[];
	isSubmitting?: boolean;
	placeholder?: string;
	isEditable?: boolean;
	mode?: PlateModes;
}
