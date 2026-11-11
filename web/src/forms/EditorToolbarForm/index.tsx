import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Formik, FormikHelpers } from 'formik';
import { useCallback } from 'react';

import { getAddingPopUpTitle } from '@/constants/activityStatus';
import { TextField } from '@/fields/Text';
import {
	EDITOR_POPUP_Z_INDEX,
	initialFormValues,
} from '@/forms/EditorToolbarForm/constants';
import { validationSchema } from '@/forms/EditorToolbarForm/schema';
import {
	EditorToolbarFormProps,
	EditorToolbarFormValues,
} from '@/forms/EditorToolbarForm/types';
import { FormWrapper } from '@/forms/styles';

export function EditorToolbarForm({
	type,
	handleClose,
	handleSubmit,
}: EditorToolbarFormProps) {
	const handleFormSubmit = useCallback(
		async (
			values: EditorToolbarFormValues,
			{ setSubmitting }: FormikHelpers<EditorToolbarFormValues>
		) => {
			try {
				setSubmitting(true);

				await handleSubmit(values);
			} finally {
				setSubmitting(false);
			}
		},
		[handleSubmit]
	);

	return (
		<Formik
			enableReinitialize
			initialValues={initialFormValues}
			validationSchema={validationSchema}
			onSubmit={handleFormSubmit}
			validateOnBlur={false}
		>
			{({ submitForm, isSubmitting, setFieldValue }) => (
				<PopUp
					type="form"
					showLoader={isSubmitting}
					title={getAddingPopUpTitle(type)}
					onClose={handleClose}
					controls={{
						positiveControl: { onClick: submitForm },
						negativeControl: { onClick: handleClose },
					}}
					zIndex={EDITOR_POPUP_Z_INDEX}
				>
					<FormWrapper>
						<TextField
							name="url"
							label={`${type} url`}
							onChange={setFieldValue}
							isRequired
						/>
					</FormWrapper>
				</PopUp>
			)}
		</Formik>
	);
}
