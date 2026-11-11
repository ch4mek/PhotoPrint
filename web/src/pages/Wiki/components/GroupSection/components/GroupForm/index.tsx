import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { FormikHelpers } from 'formik';
import { useCallback, useMemo } from 'react';

import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import { TextField } from '@/fields/Text';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { FormWrapper } from '@/forms/styles';
import {
	initialFormValues,
	FORM_NAME,
} from '@/pages/Wiki/components/GroupSection/components/GroupForm/constants';
import { useGetKnowledgeGroupById } from '@/queries/wiki/useGetKnowledgeGroupById';

import { getValidationSchema } from './schema';
import { GroupDataFormProps, GroupFormValues } from './types';

export function GroupForm({
	type,
	handleClose,
	handleSubmit,
	groupId,
}: GroupDataFormProps) {
	const isEditMode = groupId != null;

	const { data, isLoading, isFetching } = useGetKnowledgeGroupById(groupId, {
		enabled: isEditMode,
	});

	const validationSchema = useMemo(() => getValidationSchema(type), [type]);

	const initialFormData = useMemo(() => {
		if (isEditMode && data) {
			return {
				name: data.name,
			};
		}

		return initialFormValues;
	}, [isEditMode, data]);

	const handleFormSubmit = useCallback(
		async (
			values: GroupFormValues,
			{ setSubmitting }: FormikHelpers<GroupFormValues>
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
		<FormikWithBackup
			enableReinitialize
			initialValues={initialFormData}
			validationSchema={validationSchema}
			onSubmit={handleFormSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={FORM_NAME}
			id={groupId}
		>
			{({ submitForm, isSubmitting, setFieldValue, handleOutsideClose }) => (
				<PopUp
					type="form"
					showLoader={isSubmitting || isLoading || isFetching}
					title={
						isEditMode ? getEditingPopUpTitle(type) : getAddingPopUpTitle(type)
					}
					onClose={handleClose}
					onShadowClick={handleOutsideClose}
					controls={{
						positiveControl: { onClick: submitForm },
						negativeControl: { onClick: handleClose },
					}}
				>
					<FormWrapper>
						<TextField
							name="name"
							label={`${type} name`}
							onChange={setFieldValue}
							isRequired
						/>
					</FormWrapper>
				</PopUp>
			)}
		</FormikWithBackup>
	);
}
