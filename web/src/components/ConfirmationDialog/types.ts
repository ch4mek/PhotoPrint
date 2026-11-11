export interface ConfirmationDialogProps {
	handleClose: VoidFunction;
	handleSubmit: VoidFunction;
	title: string;
	confirmationMessage: string;
	contentText?: string;
	isLoading?: boolean;
}
