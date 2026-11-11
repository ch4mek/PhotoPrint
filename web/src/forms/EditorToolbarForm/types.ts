import { MediaType } from '@/types/plateEditor';

export interface EditorToolbarFormProps {
	type: MediaType;
	handleClose: VoidFunction;
	handleSubmit: (data: EditorToolbarFormValues) => void;
}

export interface EditorToolbarFormValues {
	url: string;
}
