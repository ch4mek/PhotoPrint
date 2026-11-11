import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useMemo } from 'react';

import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { FormWrapper } from '@/forms/styles';
import dayjs, { Dayjs } from '@/lib/dayjs';

import {
	CONFIRMATION_DATE_INNER_LABEL,
	DEVELOPMENT_PLAN_APPROVING_POPUP_TITLE,
	FORM_NAME,
} from './constants';
import { developmentPlanApprovingValidationSchema } from './schema';
import {
	DevelopmentPlanApprovingFormField,
	DevelopmentPlanApprovingFormProps,
	DevelopmentPlanApprovingFormValues,
} from './types';

export function DevelopmentPlanApprovingForm({
	startDate,
	handleClose,
	onSubmit,
}: DevelopmentPlanApprovingFormProps) {
	const initialValues = useMemo<DevelopmentPlanApprovingFormValues>(
		() => ({
			startDate: new Date(startDate),
			confirmationDate: null,
		}),
		[startDate]
	);

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={initialValues}
			validationSchema={developmentPlanApprovingValidationSchema}
			onSubmit={onSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={FORM_NAME}
		>
			{({
				setFieldValue,
				setFieldTouched,
				submitForm,
				errors,
				touched,
				values: { confirmationDate },
				handleOutsideClose,
			}) => {
				const handleFieldChange =
					(fieldName: string) => (value: Dayjs | null) => {
						if (value === null) {
							setFieldValue(fieldName, null, true);

							return;
						}

						setFieldValue(fieldName, value, true);
						setFieldTouched(fieldName, true);
					};

				return (
					<PopUp
						title={DEVELOPMENT_PLAN_APPROVING_POPUP_TITLE}
						onClose={handleClose}
						onShadowClick={handleOutsideClose}
						controls={{
							positiveControl: { onClick: submitForm },
							negativeControl: { onClick: handleClose },
						}}
						type="form"
					>
						<FormWrapper>
							<DatePicker
								innerLabel={CONFIRMATION_DATE_INNER_LABEL}
								errorText={
									touched.confirmationDate && errors.confirmationDate
										? String(errors.confirmationDate)
										: ''
								}
								size="fullWidth"
								value={dayjs(confirmationDate)}
								onChange={handleFieldChange(
									DevelopmentPlanApprovingFormField.ConfirmationDate
								)}
								isRequired
							/>
						</FormWrapper>
					</PopUp>
				);
			}}
		</FormikWithBackup>
	);
}
