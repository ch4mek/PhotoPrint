import { useTheme } from '@emotion/react';
import { CurrencyInput } from '@mms/mms-ui-library/components/CurrencyInput';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { FormikHelpers } from 'formik';
import numeral from 'numeral';
import { useCallback, useMemo } from 'react';

import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import {
	MAX_DATE_DAY,
	MAX_DATE_MONTH,
	MAX_DATE_YEAR,
	MIN_DATE_DAY,
	MIN_DATE_MONTH,
	MIN_DATE_YEAR,
	YYYY_MM_DD_DAYJS_DATE_FORMAT,
} from '@/constants/date';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { FormWrapper } from '@/forms/styles';
import { getDateFormat } from '@/helpers/getDateFormat';
import dayjs, { Dayjs } from '@/lib/dayjs';
import { useUserSalary } from '@/queries/teamboard/useUserSalary';

import {
	CURRENCIES_OPTIONS,
	entityName,
	MAX_SALARY_LENGTH,
	FORM_NAME,
} from './constants';
import { validationSchema } from './schema';
import { FormContent } from './styles';
import {
	UserSalaryFields,
	UserSalaryFormProps,
	UserSalaryFormValues,
} from './types';

export function UserSalaryForm({
	handleClose,
	onSubmit,
	userId,
	id,
	getUserSalaryById,
	conversationId,
}: UserSalaryFormProps) {
	const theme = useTheme();

	const { data: userSalaryData, isFetching } = useUserSalary(id, {
		enabled: Boolean(id && (!getUserSalaryById || !getUserSalaryById(id))),
	});

	const handleSubmit = useCallback(
		async (
			{ date, salaryUsd }: UserSalaryFormValues,
			{ setSubmitting }: FormikHelpers<UserSalaryFormValues>
		) => {
			const isSubmited = await onSubmit({
				conversationId,
				date: getDateFormat(date, YYYY_MM_DD_DAYJS_DATE_FORMAT),
				salaryUsd: numeral(salaryUsd).value(),
				userId,
				id,
			});

			setSubmitting(false);

			if (isSubmited) {
				handleClose();
			}
		},
		[userId, id]
	);

	const initialValues: UserSalaryFormValues = useMemo(() => {
		if (getUserSalaryById && id) {
			return getUserSalaryById(id);
		}

		if (userSalaryData) {
			const { salaryFrom, salaryUsd } = userSalaryData;

			return {
				date: dayjs(salaryFrom),
				salaryUsd: salaryUsd.toString(),
			};
		}

		return {
			date: null,
			salaryUsd: '',
		};
	}, [userSalaryData, getUserSalaryById, id]);

	const [minDate, maxDate] = useMemo(
		() => [
			dayjs([MIN_DATE_YEAR, MIN_DATE_MONTH, MIN_DATE_DAY]),
			dayjs([MAX_DATE_YEAR, MAX_DATE_MONTH, MAX_DATE_DAY]),
		],
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
			name={FORM_NAME}
			id={`${userId}_${id}`}
		>
			{({
				setFieldValue,
				setFieldTouched,
				submitForm,
				isSubmitting,
				values,
				touched,
				errors,
				handleOutsideClose,
			}) => {
				const handleDateChange =
					(fieldName: UserSalaryFields.date) => (value: Dayjs | null) => {
						setFieldValue(fieldName, value, true);
						setFieldTouched(fieldName, true);
					};

				const handleInputSalaryValueChange =
					(fieldName: UserSalaryFields.salaryUsd) => (value: string) => {
						setFieldValue(fieldName, value, true);
						setFieldTouched(fieldName, true);
					};

				return (
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
						showLoader={isSubmitting || isFetching}
					>
						<FormWrapper>
							<FormContent>
								<DatePicker
									innerLabel="Date"
									size="fullWidth"
									errorText={touched.date ? errors.date : ''}
									value={dayjs(values.date)}
									onChange={handleDateChange(UserSalaryFields.date)}
									minDate={minDate}
									maxDate={maxDate}
									isRequired
								/>
								<CurrencyInput
									size="fullWidth"
									label="Salary"
									maxLength={MAX_SALARY_LENGTH}
									value={values.salaryUsd}
									error={
										Boolean(errors.salaryUsd) && Boolean(touched.salaryUsd)
									}
									errorText={errors.salaryUsd}
									currency={CURRENCIES_OPTIONS[0]}
									currenciesOptions={CURRENCIES_OPTIONS}
									disabledCurrencyChange
									dropdownLabelText={theme.palette['text-gray']}
									onValueChange={handleInputSalaryValueChange(
										UserSalaryFields.salaryUsd
									)}
									includeDecimalPart={false}
									isRequired
								/>
							</FormContent>
						</FormWrapper>
					</PopUp>
				);
			}}
		</FormikWithBackup>
	);
}
