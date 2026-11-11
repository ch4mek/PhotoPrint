import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { FormikHelpers } from 'formik';
import { useContext, useMemo } from 'react';

import {
	getAddingPopUpTitle,
	getEditingPopUpTitle,
} from '@/constants/activityStatus';
import { DateFormat } from '@/constants/date';
import { useConfiguration } from '@/context/Configuration';
import { TextField } from '@/fields/Text';
import { TextArea } from '@/fields/TextArea';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { FormWrapper } from '@/forms/styles';
import dayjs, { Dayjs } from '@/lib/dayjs';
import { PersonalDataContext } from '@/pages/TeamBoardUser/context/personalDataContext';
import { useGetContractById } from '@/queries/teamboard/contracts/useGetContractById';

import {
	ENTITY_NAME,
	USER_CONTRACTS_FORM_FIELDS,
	FORM_NAME,
} from './constants';
import { validationSchema } from './schema';
import { DatePickersWrapper } from './styles';
import { UserContractsFormFields, UserContractsFormProps } from './types';

export function UserContractsForm({
	id,
	handleClose,
	onUpdate,
	onCreate,
	isContractOnly = false,
}: UserContractsFormProps) {
	const IS_EDIT_MODE = Boolean(id);
	const { userId } = useContext(PersonalDataContext);
	const {
		persistentValues: { fireTypes },
	} = useConfiguration();

	const { data, isLoading } = useGetContractById(id);

	const initialValues = useMemo(
		() => ({
			...data,
			startDate: data?.contractFrom ? dayjs(data.contractFrom) : null,
			endDate: data?.contractTo ? dayjs(data.contractTo) : null,
			fireDate: data?.fireDate ? dayjs(data.fireDate) : null,
			fireType:
				fireTypes.find(({ id: fireTypeId }) => fireTypeId === data?.fireType) ??
				null,
			fireReason: data?.fireReason ?? '',
			fireDescription: data?.fireDescription ?? '',
		}),
		[data]
	);

	const handleSubmit = async (
		values: UserContractsFormFields,
		{ setSubmitting }: FormikHelpers<UserContractsFormFields>
	) => {
		const {
			startDate,
			endDate,
			fireDate,
			fireType,
			fireReason,
			fireDescription,
		} = values;

		if (!startDate) {
			return;
		}

		const formattedValues = {
			contractFrom: dayjs(startDate).format(DateFormat.YYYY_MM_DD),
			contractTo: endDate ? dayjs(endDate).format(DateFormat.YYYY_MM_DD) : null,
			fireDate: fireDate ? dayjs(fireDate).format(DateFormat.YYYY_MM_DD) : null,
			fireType: fireType?.name ?? null,
			fireReason: fireReason ?? '',
			fireDescription: fireDescription ?? '',
		};

		if (IS_EDIT_MODE && onUpdate) {
			const isSubmited = await onUpdate({
				contractId: id,
				...formattedValues,
			});

			if (isSubmited) {
				handleClose();
			}
		} else if (userId) {
			const isSubmited = await onCreate({ userId, ...formattedValues });

			if (isSubmited) {
				handleClose();
			}
		}

		setSubmitting(false);
	};

	return (
		<FormikWithBackup
			initialValues={initialValues}
			enableReinitialize
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
				values,
				touched,
				errors,
				isSubmitting,
				validateField,
				handleOutsideClose,
			}) => {
				const handleDateChange =
					(fieldName: string) => (value: Dayjs | null) => {
						setFieldValue(fieldName, value, true);
						setFieldTouched(fieldName, true);
					};

				const handleOnChange = (field: string) => (value: unknown) => {
					setFieldValue(field, value);
					setFieldTouched(field, true);
					validateField(field);
				};

				const handleTextFieldChange = (fieldName: string, value: unknown) => {
					setFieldValue(fieldName, value);
					setFieldTouched(fieldName, true);
					validateField(fieldName);
				};

				return (
					<PopUp
						type="form"
						showLoader={isLoading || isSubmitting}
						title={
							IS_EDIT_MODE
								? getEditingPopUpTitle(ENTITY_NAME)
								: getAddingPopUpTitle(ENTITY_NAME)
						}
						onClose={handleClose}
						onShadowClick={handleOutsideClose}
						controls={{
							positiveControl: {
								onClick: submitForm,
								disabled: isSubmitting,
							},
							negativeControl: { onClick: handleClose },
						}}
					>
						<FormWrapper>
							<DatePickersWrapper>
								<DatePicker
									innerLabel={USER_CONTRACTS_FORM_FIELDS.startDate.label}
									size="fullWidth"
									errorText={
										touched.startDate && errors.startDate
											? String(errors.startDate)
											: ''
									}
									value={dayjs(values.startDate)}
									onChange={handleDateChange(
										USER_CONTRACTS_FORM_FIELDS.startDate.name
									)}
									isRequired
								/>
								<DatePicker
									innerLabel={USER_CONTRACTS_FORM_FIELDS.endDate.label}
									size="fullWidth"
									errorText={
										touched.endDate && errors.endDate
											? String(errors.endDate)
											: ''
									}
									value={dayjs(values.endDate)}
									onChange={handleDateChange(
										USER_CONTRACTS_FORM_FIELDS.endDate.name
									)}
									isRequired
								/>
							</DatePickersWrapper>
							{!isContractOnly && (
								<>
									<DatePicker
										innerLabel={USER_CONTRACTS_FORM_FIELDS.fireDate.label}
										size="fullWidth"
										errorText={
											touched.fireDate && errors.fireDate
												? String(errors.fireDate)
												: ''
										}
										value={dayjs(values.fireDate)}
										onChange={handleDateChange(
											USER_CONTRACTS_FORM_FIELDS.fireDate.name
										)}
									/>
									<Autoselect
										{...USER_CONTRACTS_FORM_FIELDS.fireType}
										valueField="description"
										value={values.fireType}
										options={fireTypes}
										error={Boolean(errors.fireType) && touched.fireType}
										errorText={errors.fireType}
										onChange={handleOnChange(
											USER_CONTRACTS_FORM_FIELDS.fireType.name
										)}
									/>
									<TextField
										{...USER_CONTRACTS_FORM_FIELDS.fireReason}
										onChange={handleTextFieldChange}
										value={values?.fireReason ?? ''}
									/>
									<TextArea
										{...USER_CONTRACTS_FORM_FIELDS.fireDescription}
										size="fullWidth"
										onChange={handleTextFieldChange}
									/>
								</>
							)}
						</FormWrapper>
					</PopUp>
				);
			}}
		</FormikWithBackup>
	);
}
