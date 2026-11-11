import { Dispatch, ReactNode, SetStateAction } from 'react';

import { PlateModes } from '../../constants';

import { AppFile } from '@/types/file';
import { CustomEditor, CustomValue } from '@/types/plateEditor';

export interface EditorFormProps {
	editorId: string;
	editorData: CustomValue;
	files: AppFile[];
	onSetFiles: Dispatch<SetStateAction<AppFile[]>>;
	isSubmitting: boolean;
	isEditable: boolean;
	mode: PlateModes;
	onEditorChange: (value: CustomValue) => void;
	onSubmit: (text: string, files: AppFile[]) => Promise<void>;
	onReset: VoidFunction;
	children: (props: { isSendDisabled: boolean }) => ReactNode;
}

export type UseEditorFormHandlersParams = {
	editor: CustomEditor | null;
	editorData: CustomValue;
	files: AppFile[];
	isSubmitting: boolean;
	onSubmit: (text: string, files: AppFile[]) => Promise<void>;
	onReset: VoidFunction;
	mode: PlateModes;
};

export type EditorReadyNotifierProps = {
	onReady: (editor: CustomEditor) => void;
};
