import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { FormikHelpers } from 'formik';
import { useCallback } from 'react';

import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { FormWrapper } from '@/forms/styles';
import dayjs, { Dayjs } from '@/lib/dayjs';
import { EmploymentHistoryFields as EmploymentHistoryLabels } from '@/pages/TeamBoardUser/components/EmploymentHistory/constants';

import { EmploymentHistoryFields, POPUP_TITLE } from './constants';
import { validationSchema } from './schema';
import { FormRow } from './styles';
import {
	EmploymentHistoryFormProps,
	EmploymentHistoryFormValues,
} from './types';

export function EmploymentHistoryForm({
	handleClose,
	onSubmit,
	id,
	initialValues,
	shouldDisplayNda = false,
}: EmploymentHistoryFormProps) {
	const handleSubmit = useCallback(
		async (
			values: EmploymentHistoryFormValues,
			{
				setSubmitting,
				setFieldTouched,
			}: FormikHelpers<EmploymentHistoryFormValues>
		) => {
			const isSubmited = await onSubmit({ ...values, userId: id });

			setSubmitting(false);
			setFieldTouched(
				EmploymentHistoryLabels.PROFESSIONAL_ACTIVITY_START_DATE,
				false
			);

			if (isSubmited) {
				handleClose();
			}
		},
		[]
	);

	return (
		<FormikWithBackup
			enableReinitialize
			onSubmit={handleSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={POPUP_TITLE}
			{...{ initialValues, validationSchema, id }}
		>
			{({
				setFieldValue,
				submitForm,
				isSubmitting,
				setFieldTouched,
				touched,
				values,
				errors,
				handleOutsideClose,
			}) => {
				const {
					professionalActivityStartDate,
					hireDate,
					fireDate,
					ndaStartDate,
					ndaEndDate,
				} = values;

				const handleDateChange =
					(fieldName: EmploymentHistoryFields) => (value: Dayjs | null) => {
						setFieldValue(fieldName, value, true);
						setFieldTouched(fieldName, true);
					};

				return (
					<PopUp
						title={POPUP_TITLE}
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
							<DatePicker
								innerLabel={
									EmploymentHistoryLabels.PROFESSIONAL_ACTIVITY_START_DATE
								}
								size="fullWidth"
								errorText={
									touched.professionalActivityStartDate
										? errors.professionalActivityStartDate
										: ''
								}
								value={dayjs(professionalActivityStartDate)}
								onChange={handleDateChange(
									EmploymentHistoryFields.PROFESSIONAL_ACTIVITY_START_DATE
								)}
							/>
							<DatePicker
								innerLabel={EmploymentHistoryLabels.EMPLOYMENT_DATE}
								size="fullWidth"
								errorText={
									touched.hireDate || touched.fireDate ? errors.hireDate : ''
								}
								value={dayjs(hireDate)}
								onChange={handleDateChange(EmploymentHistoryFields.HIRE_DATE)}
								isRequired
							/>
							{shouldDisplayNda && (
								<FormRow>
									<DatePicker
										innerLabel={EmploymentHistoryLabels.NDA_START_DATE}
										size="fullWidth"
										errorText={touched.ndaStartDate ? errors.ndaStartDate : ''}
										value={dayjs(ndaStartDate)}
										onChange={handleDateChange(
											EmploymentHistoryFields.NDA_START_DATE
										)}
									/>
									<DatePicker
										innerLabel={EmploymentHistoryLabels.NDA_END_DATE}
										size="fullWidth"
										errorText={touched.ndaEndDate ? errors.ndaEndDate : ''}
										value={dayjs(ndaEndDate)}
										onChange={handleDateChange(
											EmploymentHistoryFields.NDA_END_DATE
										)}
									/>
								</FormRow>
							)}
							<DatePicker
								innerLabel={EmploymentHistoryLabels.FIRE_DATE}
								size="fullWidth"
								errorText={
									touched.fireDate || touched.hireDate ? errors.fireDate : ''
								}
								value={dayjs(fireDate)}
								onChange={handleDateChange(EmploymentHistoryFields.FIRE_DATE)}
							/>
						</FormWrapper>
					</PopUp>
				);
			}}
		</FormikWithBackup>
	);
}
