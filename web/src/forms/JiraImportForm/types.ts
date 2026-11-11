import { FormikWithBackupRenderProps } from '@/forms/components/FormikWithBackup/types';
import { JiraImportProjectOption } from '@/types/jiraImport';

export interface InitialDataValues {
	project: JiraImportProjectOption | null;
	dateFrom: Date | null;
	dateTo: Date | null;
}

export type JiraImportFormValues = {
	[Key in keyof InitialDataValues]: NonNullable<InitialDataValues[Key]>;
};

export interface JiraImportFormProps {
	handleClose: VoidFunction;
}

export interface JiraImportFormContentProps {
	formikProps: FormikWithBackupRenderProps<InitialDataValues>;
	isDisableForm?: boolean;
	handleClose: VoidFunction;
}
