import { ReactNode } from 'react';

export interface DeleteConfirmationDialogProps {
	title: string;
	handleClose: VoidFunction;
	handleSubmit: VoidFunction;
	isLoading: boolean;
	showConfirmationMessage?: boolean;
	item?: string;
	customTitle?: boolean;
	content?: ReactNode;
}
