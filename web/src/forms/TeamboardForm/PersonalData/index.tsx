import { useTheme } from '@emotion/react';
import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { DatePicker } from '@mms/mms-ui-library/components/CustomDatePicker';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { AxiosError } from 'axios';
import { FormikHelpers } from 'formik';
import { useCallback, useContext, useMemo, useState } from 'react';

import { UserContractsForm } from '../UserContracts';

import { currentDate } from '@/constants/date';
import { Departments } from '@/constants/departments';
import {
	getIsAlreadyUsedMessage,
	RESPONSE_EMAIL_EXIST,
} from '@/constants/errors';
import { ONLY_DIGITS_AND_PLUS_REG_EXP } from '@/constants/regexp';
import {
	isChiefRole,
	isGroupManager,
	isManagerRole,
	isNotChiefRole,
	isRoleWithGroup,
	Roles,
	isIntern,
	isSysAdmin,
} from '@/constants/roles';
import {
	EMAIL_FIELDS,
	graduateJobPlacementOptions,
	militaryServiceStatusOptions,
} from '@/constants/user';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { useToast } from '@/context/Toast';
import { TextField } from '@/fields/Text';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { FormWrapper } from '@/forms/styles';
import {
	checkIfMessageIsAboutEmailDuplicate,
	checkIfMessageIsAboutPhoneDuplicate,
} from '@/helpers/checkIfMessageIsAboutDuplicate';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { filterAvailableRoles } from '@/helpers/filterAvailableRoles';
import { getEmailDuplicateFields } from '@/helpers/getEmailDuplicateFields';
import { useResize } from '@/hooks/useResize';
import dayjs, { Dayjs } from '@/lib/dayjs';
import { useContractsMutation } from '@/pages/TeamBoardUser/components/ContractsTab/hooks/useContractsMutation';
import { PersonalDataContext } from '@/pages/TeamBoardUser/context/personalDataContext';
import { isInternProfile } from '@/pages/TeamBoardUser/utils';
import { useGetDepartments } from '@/queries/departments/useGetDepartments';
import {
	CreateUserContract,
	UpdateUserContract,
} from '@/queries/teamboard/types';
import { ServerResponseError } from '@/queries/types';
import { useGetUserContracts } from '@/queries/useGetUserContracts';
import { UserClaims } from '@/types/claims';
import {
	ComplexConfigurableConfigurationItem,
	PersistentConfigurationItem,
} from '@/types/configuration';
import { DepartmentGroup } from '@/types/department';

import {
	PERSONAL_DATA_FORM_FIELDS,
	POPUP_TITLE,
	FORM_NAME,
	GROUP_FIELD_DISABLED_TEXT,
	DEPARTMENT_FIELD_DISABLED_TEXT,
	roleRestrictions,
	CONTRACTS_SKIP,
	CONTRACTS_TOP,
	CONTRACTS_SORT_ORDER,
	CONTRACTS_SORT_BY,
	EMPTY_CONTRACTS_FILTER,
} from './constants';
import { getValidationSchema } from './schema';
import { FormRow } from './styles';
import { PersonalDataFormProps, PersonalDataFormValues } from './types';
import {
	getPersonalDataSubmitPayload,
	getPopUpStylesConfig,
	withTooltip,
} from './utils';

export function PersonalDataForm({
	handleClose,
	onSubmit,
	id,
	initialValues,
	hasFullAccess,
}: PersonalDataFormProps) {
	const toast = useToast();
	const {
		data: { email },
		position,
		profileKind,
	} = useContext(PersonalDataContext);
	const isInternUser = isInternProfile(profileKind);
	const shouldLoadContracts =
		isInternUser && isIntern(initialValues.position?.id);

	const [pendingUserData, setPendingUserData] =
		useState<PersonalDataFormValues | null>(null);
	const [isContractModalOpen, setIsContractModalOpen] = useState(false);

	const { data: userContractsData } = useGetUserContracts(
		id,
		toast,
		CONTRACTS_SKIP,
		CONTRACTS_TOP,
		CONTRACTS_SORT_ORDER,
		CONTRACTS_SORT_BY,
		EMPTY_CONTRACTS_FILTER,
		undefined,
		{ enabled: shouldLoadContracts }
	);

	const latestContract = userContractsData?.items?.[0];

	const hasFilledContract = useMemo(
		() =>
			userContractsData?.items?.some(
				({ contractFrom, contractTo }) => contractFrom && contractTo
			),

		[userContractsData]
	);

	const { createContract, updateContract } = useContractsMutation();

	const {
		configurableValues: { englishLevels, departmentTypes, roleTypes },
	} = useConfiguration();

	const theme = useTheme();
	const [, windowHeight] = useResize();
	const { role, claims } = useAuth();

	const { data: departmentsInfo } = useGetDepartments();

	const hasManageHRClaim = checkIsHasClaims(
		[UserClaims.ManageHRModule],
		claims
	);
	const isChief = isChiefRole(role);

	const canEditInternPosition = hasManageHRClaim || isChief;

	const handleContractModalClose = useCallback(() => {
		setIsContractModalOpen(false);
		setPendingUserData(null);
	}, []);

	const submitPersonalData = useCallback(
		async (userData: PersonalDataFormValues) => {
			const isSubmitted = await onSubmit(
				getPersonalDataSubmitPayload(userData, id)
			);

			if (isSubmitted) {
				handleContractModalClose();
				handleClose();
			}
		},
		[onSubmit, id, handleContractModalClose, handleClose]
	);

	const handleContractCreate = useCallback(
		async (contractData: CreateUserContract): Promise<object> => {
			if (!pendingUserData) {
				return {};
			}

			await createContract(contractData);
			await submitPersonalData(pendingUserData);

			return {};
		},
		[pendingUserData, createContract, submitPersonalData]
	);

	const handleContractUpdate = useCallback(
		async (contractData: UpdateUserContract): Promise<object> => {
			if (!pendingUserData) {
				return {};
			}

			await updateContract(contractData);
			await submitPersonalData(pendingUserData);

			return {};
		},
		[pendingUserData, updateContract, submitPersonalData]
	);

	const handleSubmit = useCallback(
		async (
			values: PersonalDataFormValues,
			{ setSubmitting, setFieldError }: FormikHelpers<PersonalDataFormValues>
		) => {
			const wasOriginallyIntern = isIntern(initialValues.position?.id);
			const isCurrentlyIntern = isIntern(values.position?.id);
			const isPromotedFromIntern = wasOriginallyIntern && !isCurrentlyIntern;

			if (isPromotedFromIntern && !hasFilledContract) {
				setPendingUserData(values);
				setIsContractModalOpen(true);
				setSubmitting(false);

				return;
			}

			try {
				const isSubmited = await onSubmit(
					getPersonalDataSubmitPayload(values, id)
				);

				setSubmitting(false);

				if (isSubmited) {
					handleClose();
				}
			} catch (error) {
				const message = (
					error as AxiosError<ServerResponseError>
				)?.response?.data?.Message?.trim();

				if (checkIfMessageIsAboutEmailDuplicate(message)) {
					getEmailDuplicateFields(message, values, EMAIL_FIELDS).forEach(
						(field) => setFieldError(field, RESPONSE_EMAIL_EXIST)
					);
				}

				if (checkIfMessageIsAboutPhoneDuplicate(message)) {
					const { name, label } = PERSONAL_DATA_FORM_FIELDS.personalPhone;
					setFieldError(name, getIsAlreadyUsedMessage(label, false));
				}
			}
		},
		[initialValues.position, hasFilledContract]
	);

	const availableRoles = useMemo(
		() => filterAvailableRoles(roleTypes, role, roleRestrictions),
		[roleTypes, role]
	);

	const popUpStylesConfig = useMemo(
		() => getPopUpStylesConfig(theme, windowHeight),
		[theme, windowHeight]
	);

	const validationSchema = useMemo(() => getValidationSchema(), []);

	const canEditEmployeeDepartment = isChiefRole(role);

	const shouldEnableGroupField = isManagerRole(role);

	const shouldDisablePositionField = isInternUser
		? !canEditInternPosition
		: !hasFullAccess && hasManageHRClaim;

	const positionOptions = useMemo(() => {
		if (isInternUser && hasManageHRClaim) {
			return availableRoles.filter(
				({ id: roleId }) => roleId === Roles.SE || roleId === Roles.IN
			);
		}

		return availableRoles;
	}, [availableRoles, isInternUser, hasManageHRClaim]);

	const ldDepartment = useMemo(
		() =>
			departmentTypes.find(
				({ id: departmentTypeId }) => departmentTypeId === Departments.LD
			) ?? null,
		[departmentTypes]
	);

	return (
		<>
			<FormikWithBackup
				enableReinitialize
				onSubmit={handleSubmit}
				onClose={handleClose}
				validateOnBlur={false}
				name={FORM_NAME}
				{...{ initialValues, validationSchema, id }}
			>
				{({
					setFieldValue,
					setFieldTouched,
					validateField,
					submitForm,
					isSubmitting,
					errors,
					touched,
					values,
					handleOutsideClose,
				}) => {
					const handleOnChange = (field: string) => (value: unknown) => {
						setFieldValue(field, value);
						setFieldTouched(field, true);
						validateField(field);
					};

					const handleDateChange =
						(field: string) => (timestamp: Dayjs | null) => {
							setFieldValue(field, timestamp);
							setFieldTouched(field, true);
							validateField(field);
						};

					const handlePhoneNumberChange = (
						fieldName: string,
						fieldValue: string
					) => {
						const filteredValue = fieldValue.replace(
							ONLY_DIGITS_AND_PLUS_REG_EXP,
							''
						);

						if (filteredValue === fieldValue) {
							setFieldValue(fieldName, filteredValue);
							validateField(fieldName);
							setFieldTouched(fieldName, true);
						}
					};

					const handlePositionChange = async (
						option: PersistentConfigurationItem | null
					) => {
						setFieldValue(
							PERSONAL_DATA_FORM_FIELDS.position.name,
							option,
							false
						);
						setFieldTouched(PERSONAL_DATA_FORM_FIELDS.position.name, true);
						await validateField(PERSONAL_DATA_FORM_FIELDS.position.name);

						setFieldTouched(PERSONAL_DATA_FORM_FIELDS.group.name, false);
						setFieldValue(PERSONAL_DATA_FORM_FIELDS.group.name, null);

						if (option && isIntern(option.name as Roles)) {
							setFieldValue(
								PERSONAL_DATA_FORM_FIELDS.department.name,
								ldDepartment,
								false
							);
						}

						await validateField(PERSONAL_DATA_FORM_FIELDS.personalEmail.name);
						await validateField(PERSONAL_DATA_FORM_FIELDS.corporateEmail.name);
					};

					const handleDepartmentChange = async (
						option: ComplexConfigurableConfigurationItem | null
					) => {
						setFieldValue(PERSONAL_DATA_FORM_FIELDS.department.name, option);
						setFieldTouched(PERSONAL_DATA_FORM_FIELDS.department.name, true);
						await validateField(PERSONAL_DATA_FORM_FIELDS.department.name);

						setFieldTouched(PERSONAL_DATA_FORM_FIELDS.group.name, false);
						setFieldValue(PERSONAL_DATA_FORM_FIELDS.group.name, null);
					};

					const departmentGroups: DepartmentGroup[] = useMemo(() => {
						const groups =
							departmentsInfo?.find(
								({ departmentName }) =>
									departmentName === values.department?.fullName
							)?.groups ?? [];

						return isGroupManager(values.position?.id as Roles)
							? groups.filter(
									({ groupManagerEmail }) =>
										!groupManagerEmail || groupManagerEmail === email
							  )
							: groups;
					}, [
						departmentsInfo,
						values.department?.fullName,
						values.position?.id,
						email,
					]);

					const shouldDisplayGroupField = isRoleWithGroup(
						values.position?.id as Roles
					);

					const wasOriginallyIntern = isIntern(initialValues.position?.id);
					const isCurrentlyIntern = isIntern(values.position?.id);
					const isPromotedFromIntern =
						wasOriginallyIntern && !isCurrentlyIntern;

					const shouldDisableCorporateEmail =
						!isSysAdmin(role) && !isPromotedFromIntern && isNotChiefRole(role);

					const renderDepartmentField = () => {
						if (isCurrentlyIntern) {
							return (
								<Autoselect
									{...PERSONAL_DATA_FORM_FIELDS.department}
									value={ldDepartment ?? values.department}
									valueField="fullName"
									options={departmentTypes}
									onChange={handleDepartmentChange}
									disabled
								/>
							);
						}

						const canEditDepartment = isInternUser
							? canEditInternPosition
							: canEditEmployeeDepartment;

						if (canEditDepartment) {
							return (
								<Autoselect
									{...PERSONAL_DATA_FORM_FIELDS.department}
									value={values.department}
									valueField="fullName"
									options={departmentTypes}
									onChange={handleDepartmentChange}
									error={Boolean(errors.department) && touched.department}
									errorText={errors.department}
									isRequired
								/>
							);
						}

						return withTooltip(
							<Autoselect
								{...PERSONAL_DATA_FORM_FIELDS.department}
								value={values.department}
								valueField="fullName"
								options={departmentTypes}
								onChange={handleDepartmentChange}
								disabled
							/>,
							DEPARTMENT_FIELD_DISABLED_TEXT
						);
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
							stylesConfig={popUpStylesConfig}
						>
							<FormWrapper>
								<FormRow>
									<TextField
										{...PERSONAL_DATA_FORM_FIELDS.lastName}
										onChange={setFieldValue}
										isRequired
									/>
									<TextField
										{...PERSONAL_DATA_FORM_FIELDS.firstName}
										onChange={setFieldValue}
										isRequired
									/>
								</FormRow>

								<FormRow>
									<TextField
										{...PERSONAL_DATA_FORM_FIELDS.lastNameRu}
										onChange={setFieldValue}
										isRequired
									/>
									<TextField
										{...PERSONAL_DATA_FORM_FIELDS.firstNameRu}
										onChange={setFieldValue}
										isRequired
									/>
									<TextField
										{...PERSONAL_DATA_FORM_FIELDS.patronymicRu}
										onChange={setFieldValue}
										isRequired
									/>
								</FormRow>

								<FormRow>
									<DatePicker
										innerLabel={PERSONAL_DATA_FORM_FIELDS.dateOfBirth.label}
										size="fullWidth"
										errorText={touched.dateOfBirth ? errors.dateOfBirth : ''}
										value={dayjs(values.dateOfBirth)}
										onChange={handleDateChange(
											PERSONAL_DATA_FORM_FIELDS.dateOfBirth.name
										)}
										maxDate={currentDate}
										isRequired
									/>
									<TextField
										{...PERSONAL_DATA_FORM_FIELDS.personalPhone}
										onChange={handlePhoneNumberChange}
									/>
									<TextField
										{...PERSONAL_DATA_FORM_FIELDS.telegram}
										onChange={setFieldValue}
									/>
								</FormRow>

								<FormRow>
									<Autoselect
										{...PERSONAL_DATA_FORM_FIELDS.militaryServiceStatus}
										value={values.militaryServiceStatus ?? null}
										onChange={handleOnChange(
											PERSONAL_DATA_FORM_FIELDS.militaryServiceStatus.name
										)}
										options={militaryServiceStatusOptions}
										error={Boolean(errors.militaryServiceStatus)}
										errorText={errors.militaryServiceStatus}
									/>
									<Autoselect
										{...PERSONAL_DATA_FORM_FIELDS.graduateJobPlacement}
										value={values.graduateJobPlacement ?? null}
										onChange={handleOnChange(
											PERSONAL_DATA_FORM_FIELDS.graduateJobPlacement.name
										)}
										options={graduateJobPlacementOptions}
										error={Boolean(errors.graduateJobPlacement)}
										errorText={errors.graduateJobPlacement}
									/>
								</FormRow>

								<FormRow>
									<Autoselect
										label={PERSONAL_DATA_FORM_FIELDS.position.label}
										value={values.position}
										valueField="description"
										options={positionOptions}
										disabled={shouldDisablePositionField}
										onChange={handlePositionChange}
										error={Boolean(errors.position) && touched.position}
										errorText={errors.position}
										isRequired
									/>
									{!isChiefRole(position) && renderDepartmentField()}
									{shouldDisplayGroupField &&
										(shouldEnableGroupField ? (
											<Autoselect
												{...PERSONAL_DATA_FORM_FIELDS.group}
												value={values.group?.name}
												valueField="name"
												options={departmentGroups}
												onChange={handleOnChange(
													PERSONAL_DATA_FORM_FIELDS.group.name
												)}
												error={Boolean(errors.group) && touched.group}
												errorText={errors.group}
											/>
										) : (
											withTooltip(
												<Autoselect
													{...PERSONAL_DATA_FORM_FIELDS.group}
													value={values.group?.name}
													options={departmentGroups}
													onChange={handleOnChange(
														PERSONAL_DATA_FORM_FIELDS.group.name
													)}
													disabled
												/>,
												GROUP_FIELD_DISABLED_TEXT
											)
										))}
								</FormRow>

								{!isCurrentlyIntern && (
									<TextField
										{...PERSONAL_DATA_FORM_FIELDS.corporateEmail}
										onChange={setFieldValue}
										disabled={shouldDisableCorporateEmail}
										isRequired
									/>
								)}
								<TextField
									{...PERSONAL_DATA_FORM_FIELDS.personalEmail}
									onChange={setFieldValue}
									isRequired={isCurrentlyIntern}
								/>
								<TextField
									{...PERSONAL_DATA_FORM_FIELDS.personalEmailSecond}
									onChange={setFieldValue}
								/>
								<TextField
									{...PERSONAL_DATA_FORM_FIELDS.linkedIn}
									onChange={setFieldValue}
								/>

								<FormRow>
									<Autoselect
										{...PERSONAL_DATA_FORM_FIELDS.englishLevel}
										value={values.englishLevel?.name}
										options={englishLevels}
										onChange={handleOnChange(
											PERSONAL_DATA_FORM_FIELDS.englishLevel.name
										)}
										error={Boolean(errors.englishLevel) && touched.englishLevel}
										errorText={errors.englishLevel}
										isRequired
									/>
								</FormRow>
							</FormWrapper>
						</PopUp>
					);
				}}
			</FormikWithBackup>
			{isContractModalOpen && (
				<UserContractsForm
					id={latestContract?.id ?? 0}
					handleClose={handleContractModalClose}
					onCreate={handleContractCreate}
					onUpdate={handleContractUpdate}
					isContractOnly
				/>
			)}
		</>
	);
}
