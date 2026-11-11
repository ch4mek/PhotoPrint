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
	EXPENSE_ENTITY_NAME,
	FORM_NAME,
	initialFormValues,
	popUpStylesConfig,
} from './constants';
import { validationSchema } from './schema';
import { ExpenseFormProps, ExpenseFormValues } from './types';

export function ExpenseForm({
	expenseId,
	initialValues,
	onSubmit,
	onClose,
}: ExpenseFormProps) {
	const isEditMode = expenseId != null;

	const initialFormData = useMemo(() => {
		if (initialValues) {
			return initialValues;
		}

		return initialFormValues;
	}, [initialValues]);

	const handleFormSubmit = useCallback(
		async (
			values: ExpenseFormValues,
			{ setSubmitting }: FormikHelpers<ExpenseFormValues>
		) => {
			try {
				setSubmitting(true);

				await onSubmit(values);
			} finally {
				setSubmitting(false);
			}
		},
		[onSubmit]
	);

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={initialFormData}
			validationSchema={validationSchema}
			onSubmit={handleFormSubmit}
			onClose={onClose}
			validateOnBlur={false}
			name={FORM_NAME}
			id={expenseId}
		>
			{({ submitForm, isSubmitting, setFieldValue, handleOutsideClose }) => (
				<PopUp
					type="form"
					showLoader={isSubmitting}
					title={
						isEditMode
							? getEditingPopUpTitle(EXPENSE_ENTITY_NAME)
							: getAddingPopUpTitle(EXPENSE_ENTITY_NAME)
					}
					onClose={onClose}
					onShadowClick={handleOutsideClose}
					controls={{
						positiveControl: { onClick: submitForm },
						negativeControl: { onClick: onClose },
					}}
					stylesConfig={popUpStylesConfig}
				>
					<FormWrapper>
						<TextField
							name="expense"
							label="Expense name"
							onChange={setFieldValue}
							isRequired
						/>
					</FormWrapper>
				</PopUp>
			)}
		</FormikWithBackup>
	);
}
