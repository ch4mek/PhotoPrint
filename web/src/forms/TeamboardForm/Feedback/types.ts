import { Dayjs } from '@/lib/dayjs';
import { FormAutoselectOption } from '@/types/form';

export interface FeedbackFormProps {
	handleClose: () => void;
	onSubmit: (values: FeedbackFormValues) => void;
	id?: number;
	userId: number;
}

export interface FeedbackFormValues {
	date: Dayjs | null;
	type: FormAutoselectOption<string> | null;
	project: FormAutoselectOption | null;
	feedback: string;
	actions: string;
	conversation: FormAutoselectOption | null;
	reviewer: string;
}
