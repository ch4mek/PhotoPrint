import { Counter } from '@mms/mms-ui-library/components/Counter';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { FormikHelpers } from 'formik';
import { useCallback } from 'react';

import { TextArea } from '@/fields/TextArea';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { FormikWithBackupRenderProps } from '@/forms/components/FormikWithBackup/types';
import { getStringOrFallback } from '@/helpers/getStringOrFallback';
import { useShowModal } from '@/pages/hooks/useShowModal';

import { ConfirmationDialog } from './components/ConfirmationDialog';
import {
	POPUP_TITLE,
	FORM_NAME,
	validationSchema,
	INITIAL_VALUES,
	ABSENCE_AMOUNT_MAX_VALUE,
	ABSENCE_AMOUNT_MIN_VALUE,
	ABSENCE_PRECISION,
	COUNTER_LABEL,
} from './constants';
import { CounterWrapper } from './styles';
import {
	AbsenceSettingsFormProps,
	AbsenceSettingsFormValues,
	FormContentProps,
} from './types';

function FormContent({
	values,
	setFieldValue,
	setFieldTouched,
	validateField,
	errors,
	touched,
	isLoading,
	submitForm,
	handleClose,
	isSubmitting,
	handleOutsideClose,
}: FormContentProps) {
	const handleFieldChange = useCallback(
		(fieldName: keyof AbsenceSettingsFormValues) =>
			(value: string | number) => {
				setFieldValue(fieldName, value);
				setFieldTouched(fieldName, true);
				validateField(fieldName);
			},
		[setFieldValue, setFieldTouched, validateField]
	);

	return (
		<PopUp
			title={POPUP_TITLE}
			onClose={handleClose}
			onShadowClick={handleOutsideClose}
			type="form"
			controls={{
				positiveControl: { onClick: submitForm },
				negativeControl: { onClick: handleClose },
			}}
			showLoader={isLoading || isSubmitting}
		>
			<CounterWrapper>
				<Typography variant="m-400">{COUNTER_LABEL}</Typography>
				<Counter
					value={values.amount}
					error={!!(touched.amount && errors.amount)}
					onChange={handleFieldChange('amount')}
					precision={ABSENCE_PRECISION}
					maxValue={ABSENCE_AMOUNT_MAX_VALUE}
					minValue={ABSENCE_AMOUNT_MIN_VALUE}
					editable
				/>
			</CounterWrapper>

			<TextArea
				placeholder="Comment"
				name="comment"
				onChange={(_, value) => handleFieldChange('comment')(value)}
				size="fullWidth"
				label=""
			/>
		</PopUp>
	);
}

export function AbsenceSettingsForm({
	handleClose,
	onSubmit,
	isLoading,
	userId,
	formType,
	absenceName,
}: AbsenceSettingsFormProps) {
	const {
		handleOpen: handleConfirmationOpen,
		isOpen: isConfirmationOpen,
		handleClose: handleConfirmationClose,
		id: formValues,
	} = useShowModal<AbsenceSettingsFormValues>();

	const handleFormSubmit = useCallback(
		(
			values: AbsenceSettingsFormValues,
			{ setSubmitting }: FormikHelpers<AbsenceSettingsFormValues>
		) => {
			handleConfirmationOpen(values);
			setSubmitting(false);
		},
		[handleConfirmationOpen]
	);

	const handleConfirmSubmit = useCallback(async () => {
		const { amount, comment } = formValues!;

		await onSubmit({
			daysCount: amount.toString(),
			description: getStringOrFallback(comment),
			userId,
		});

		handleConfirmationClose();
		handleClose();
	}, [formValues, onSubmit, userId, handleConfirmationClose, handleClose]);

	const renderFormContent = useCallback(
		(formikProps: FormikWithBackupRenderProps<AbsenceSettingsFormValues>) => (
			<FormContent
				{...formikProps}
				isLoading={isLoading}
				handleClose={handleClose}
			/>
		),
		[isLoading, handleClose]
	);

	return (
		<>
			<FormikWithBackup
				enableReinitialize
				initialValues={INITIAL_VALUES}
				validationSchema={validationSchema}
				validateOnBlur={false}
				onSubmit={handleFormSubmit}
				onClose={handleClose}
				name={FORM_NAME}
				id={`${userId}|${formType}|${absenceName}`}
			>
				{renderFormContent}
			</FormikWithBackup>

			{isConfirmationOpen && (
				<ConfirmationDialog
					handleClose={handleConfirmationClose}
					handleSubmit={handleConfirmSubmit}
				/>
			)}
		</>
	);
}
