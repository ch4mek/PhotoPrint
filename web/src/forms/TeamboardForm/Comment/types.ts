export interface PersonalDataFormProps {
	handleClose: () => void;
	onSubmit: (values: CommentSubmitParams) => Promise<object | undefined>;
	userId: number | string | undefined;
	initialValues: CommentFormValues;
	id: string | number | undefined;
}

export interface CommentFormValues {
	comments: string | null;
}
export interface CommentSubmitParams {
	comments: string | null;
	userId: number | string | undefined;
}
