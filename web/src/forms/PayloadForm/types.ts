export interface PayloadFormProps {
	id: number;
	handleClose: () => void;
}

export interface PayloadFormValues {
	comment: string | null;
	plan: string | null;
	needProject: boolean;
}

export interface TabWrapperProps {
	height: string;
}
