import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { FormikHelpers } from 'formik';
import { useCallback } from 'react';

import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import { TextField } from '@/fields/Text';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { FormWrapper } from '@/forms/styles';
import { useSoftSkill } from '@/queries/settings/useSoftSkill';
import { SoftSkill } from '@/types/settings';

import { entityName, formName } from './constants';
import { validationSchema } from './schema';
import { SoftSkillsFormProps, SoftSkillsFormValues } from './types';

export function SoftSkillsForm({
	initialValues,
	handleClose,
	onSubmit,
	id,
}: SoftSkillsFormProps) {
	const { data, isFetching } = useSoftSkill(id);

	const handleSubmit = useCallback(
		async (
			values: SoftSkillsFormValues,
			{ setSubmitting }: FormikHelpers<SoftSkillsFormValues>
		) => {
			const isSubmited = await onSubmit(values as unknown as SoftSkill);

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
			initialValues={data || initialValues}
			validationSchema={validationSchema}
			onSubmit={handleSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={formName}
			id={id}
		>
			{({ setFieldValue, submitForm, isSubmitting, handleOutsideClose }) => (
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
					showLoader={Boolean(isFetching && id) || isSubmitting}
				>
					<FormWrapper>
						<TextField
							name="name"
							label="Soft skill name"
							onChange={setFieldValue}
							isRequired
						/>
					</FormWrapper>
				</PopUp>
			)}
		</FormikWithBackup>
	);
}
