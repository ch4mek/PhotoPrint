import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { FormikHelpers } from 'formik';
import { useCallback } from 'react';

import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import { TextArea } from '@/fields/TextArea';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { FormWrapper } from '@/forms/styles';

import { entityName, formName } from './constants';
import { validationSchema } from './schema';
import { CommentFormValues, PersonalDataFormProps } from './types';

export function CommentForm({
	handleClose,
	onSubmit,
	userId,
	initialValues,
	id,
}: PersonalDataFormProps) {
	const handleSubmit = useCallback(
		async (
			values: CommentFormValues,
			{ setSubmitting }: FormikHelpers<CommentFormValues>
		) => {
			const isSubmited = await onSubmit({ ...values, userId });

			setSubmitting(false);

			if (isSubmited) {
				handleClose();
			}
		},
		[]
	);

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={initialValues}
			validationSchema={validationSchema}
			onSubmit={handleSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={formName}
			id={`${userId}_${id}`}
		>
			{({ submitForm, isSubmitting, handleOutsideClose }) => (
				<PopUp
					title={
						id
							? getEditingPopUpTitle(entityName)
							: getAddingPopUpTitle(entityName)
					}
					onClose={handleClose}
					onShadowClick={handleOutsideClose}
					controls={{
						positiveControl: { onClick: submitForm },
						negativeControl: { onClick: handleClose },
					}}
					type="form"
					showLoader={isSubmitting}
				>
					<FormWrapper>
						<TextArea name="comments" label="Comment" size="fullWidth" />
					</FormWrapper>
				</PopUp>
			)}
		</FormikWithBackup>
	);
}
