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
import { PlanFormValues } from '@/types/plan';

import { PLAN, FORM_NAME } from './contants';
import { isPlanEmpty } from './helpers';
import { validationSchema } from './schema';
import { PlanFormProps } from './types';

export function PlanForm({
	handleClose,
	onSubmit,
	userId,
	initialValues,
}: PlanFormProps) {
	const handleSubmit = useCallback(
		async (
			values: PlanFormValues,
			{ setSubmitting }: FormikHelpers<PlanFormValues>
		) => {
			const { plan } = values;

			const isSubmited = await onSubmit({
				...{ ...values, plan: isPlanEmpty(plan) },
				userId,
			});

			setSubmitting(false);

			if (isSubmited) {
				handleClose();
			}
		},
		[userId]
	);

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={initialValues}
			validationSchema={validationSchema}
			onSubmit={handleSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={FORM_NAME}
			id={userId}
		>
			{({ submitForm, isSubmitting, handleOutsideClose }) => (
				<PopUp
					title={
						initialValues.plan
							? getEditingPopUpTitle(PLAN)
							: getAddingPopUpTitle(PLAN)
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
						<TextArea name="plan" label={PLAN} size="fullWidth" />
					</FormWrapper>
				</PopUp>
			)}
		</FormikWithBackup>
	);
}
