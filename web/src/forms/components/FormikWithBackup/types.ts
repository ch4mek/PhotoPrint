import { FormikConfig, FormikProps, FormikValues } from 'formik';
import { ReactNode } from 'react';

export type OutsideCloseHandler = {
	handleOutsideClose: VoidFunction;
};

export type FormikWithBackupRenderProps<Values> = FormikProps<Values> &
	OutsideCloseHandler;

export type FormikWithBackupProps<
	Values extends FormikValues = FormikValues,
	ExtraProps = object
> = Omit<FormikConfig<Values>, 'children'> &
	ExtraProps & {
		name: string;
		id?: string | number | null;
		onClose: VoidFunction;
		valuesKeysExcludedFromBackup?: readonly string[];
		children?:
			| ReactNode
			| ((props: FormikWithBackupRenderProps<Values>) => ReactNode);
	};

export interface FormikWithBackupInnerProps<Values extends FormikValues> {
	formikProps: FormikProps<Values>;
	children: FormikWithBackupProps<Values>['children'];
	name: string;
	id?: string | number | null;
	onClose: VoidFunction;
	valuesKeysExcludedFromBackup?: readonly string[];
}
