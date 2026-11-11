import { Dayjs } from '@/lib/dayjs';

export interface UserSalaryFormProps {
	handleClose: () => void;
	onSubmit: (values: UpdateUserSalary) => Promise<unknown>;
	getUserSalaryById?: (idOfUserSalary: number) => UserSalaryFormValues;
	id: number;
	userId: number;
	conversationId?: number;
}

export enum UserSalaryFields {
	date = 'date',
	salaryUsd = 'salaryUsd',
}

export type UserSalaryFormValues = {
	[UserSalaryFields.date]: Dayjs | null;
	[UserSalaryFields.salaryUsd]: string;
};

export interface UpdateUserSalary {
	[UserSalaryFields.date]: string | null;
	[UserSalaryFields.salaryUsd]: number | null;
	userId: number;
	id: number;
	conversationId: number | undefined;
}
