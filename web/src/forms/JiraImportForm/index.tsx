import { FormikWithBackup } from '@/forms/components/FormikWithBackup';

import { JiraImportFormContent } from './components/JiraImportFormContent';
import { initialData, POPUP_TITLE } from './constants';
import { useJiraImportForm } from './hooks/useJiraImportForm';
import { validationSchema } from './schema';
import { JiraImportFormProps } from './types';

export function JiraImportForm({ handleClose }: JiraImportFormProps) {
	const { handleSubmit, isSubmitting } = useJiraImportForm(handleClose);

	return (
		<FormikWithBackup
			initialValues={initialData}
			validationSchema={validationSchema}
			onSubmit={handleSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={POPUP_TITLE}
			id={POPUP_TITLE}
			enableReinitialize
			validateOnChange
		>
			{(formikProps) => (
				<JiraImportFormContent
					{...{ formikProps, handleClose }}
					isDisableForm={isSubmitting}
				/>
			)}
		</FormikWithBackup>
	);
}
