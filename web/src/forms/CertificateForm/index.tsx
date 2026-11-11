import { useTheme } from '@emotion/react';
import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useQueryClient } from '@tanstack/react-query';
import { useCallback, useMemo } from 'react';

import { MAX_SELECT_YEAR, MIN_SELECT_YEAR } from '@/constants/date';
import { Departments } from '@/constants/departments';
import { CERTIFICATES_QUERY_KEY } from '@/constants/queriesKeys';
import { isNotChiefRole, Roles } from '@/constants/roles';
import { useConfiguration } from '@/context/Configuration';
import { TextField } from '@/fields/Text';
import type {
	CertificateFormProps,
	CertificateFormValues,
} from '@/forms/CertificateForm/types';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { useResize } from '@/hooks/useResize';
import dayjs, { Dayjs } from '@/lib/dayjs';
import { CreateCertificateOptions } from '@/queries/types';
import { useCreateCertificate } from '@/queries/useCreateCertificate';

import {
	MAX_DATE_DAY,
	MAX_DATE_MONTH,
	MIN_DATE_DAY,
	MIN_DATE_MONTH,
	POPUP_TITLE,
} from './constants';
import {
	certificateValidationSchema,
	certificateWithStackValidationSchema,
} from './schema';
import { FormWrapper } from './styles';
import { getPopUpStylesConfig } from './utils';

export function CertificateForm({
	initialValues,
	isPopOverVisible,
	togglePopOver,
}: CertificateFormProps) {
	const {
		configurableValues: { departmentTypes },
		persistentValues: { jsStackTypes },
	} = useConfiguration();
	const [, windowHeight] = useResize();
	const queryClient = useQueryClient();

	const createCertificateMutation = useCreateCertificate({
		onSuccess: () => {
			togglePopOver();
			queryClient.invalidateQueries({ queryKey: [CERTIFICATES_QUERY_KEY] });
		},
	});

	const theme = useTheme();

	const [minDate, maxDate] = useMemo(
		() => [
			dayjs([MIN_SELECT_YEAR, MIN_DATE_MONTH, MIN_DATE_DAY]),
			dayjs([MAX_SELECT_YEAR, MAX_DATE_MONTH, MAX_DATE_DAY]),
		],
		[]
	);

	const handleSubmit = useCallback(
		async (values: CertificateFormValues) =>
			createCertificateMutation.mutateAsync({
				...values,
				initialFirstName: initialValues.firstName,
				initialLastName: initialValues.lastName,
			} as unknown as CreateCertificateOptions),
		[isPopOverVisible]
	);

	const stackOptions = useMemo(
		() =>
			initialValues.department?.id === Departments.Js ? jsStackTypes : null,
		[initialValues.department, jsStackTypes]
	);

	const validationSchema = useMemo(
		() =>
			initialValues.department?.id === Departments.Js
				? certificateWithStackValidationSchema
				: certificateValidationSchema,
		[initialValues.department]
	);

	const formInitialValues = useMemo(
		() => ({ ...initialValues, date: dayjs().startOf('day'), stack: null }),
		[initialValues]
	);

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, windowHeight),
		[theme, windowHeight]
	);

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={formInitialValues}
			validationSchema={validationSchema}
			onSubmit={handleSubmit}
			onClose={togglePopOver}
			validateOnBlur={false}
			name={POPUP_TITLE}
			id={`${initialValues.firstName}_${initialValues.lastName}`}
		>
			{({
				setFieldValue,
				values,
				submitForm,
				errors,
				touched,
				handleOutsideClose,
			}) => {
				const handleDateChange = (newValue: Dayjs | null) => {
					if (!newValue) {
						setFieldValue('date', null);

						return;
					}

					setFieldValue('date', newValue);
				};

				const handleOnChange = (field: string) => (value: unknown) => {
					setFieldValue(field, value, true);
				};

				return (
					<PopUp
						title={POPUP_TITLE}
						onClose={togglePopOver}
						onShadowClick={handleOutsideClose}
						controls={{
							positiveControl: { onClick: submitForm },
							negativeControl: { onClick: togglePopOver },
						}}
						type="form"
						stylesConfig={popUpStylesConfig}
					>
						<FormWrapper>
							<TextField
								name="firstName"
								label="First name"
								onChange={setFieldValue}
								isRequired
							/>
							<TextField
								name="lastName"
								label="Last name"
								onChange={setFieldValue}
								isRequired
							/>
							{isNotChiefRole(values.position?.name as Roles) && (
								<Autoselect
									label="Department"
									value={values.department}
									valueField="fullName"
									options={departmentTypes}
									disabled
									onChange={handleOnChange('department')}
									isRequired
								/>
							)}
							{stackOptions && (
								<Autoselect
									label="Stack"
									value={values.stack}
									valueField="description"
									options={stackOptions}
									onChange={handleOnChange('stack')}
									error={Boolean(errors.stack) && touched.stack}
									errorText={errors.stack}
									isRequired
								/>
							)}

							<DatePicker
								innerLabel="Date"
								value={dayjs(values.date)}
								onChange={handleDateChange}
								size="fullWidth"
								errorText={errors.date ? String(errors.date) : undefined}
								minDate={minDate}
								maxDate={maxDate}
								isRequired
							/>
						</FormWrapper>
					</PopUp>
				);
			}}
		</FormikWithBackup>
	);
}
