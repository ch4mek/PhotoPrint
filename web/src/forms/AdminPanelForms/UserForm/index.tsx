import { useTheme } from '@emotion/react';
import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { Avatar } from '@mms/mms-ui-library/components/Avatar';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { FormikHelpers } from 'formik';
import {
	useCallback,
	useEffect,
	useLayoutEffect,
	useMemo,
	useRef,
	useState,
} from 'react';

import { COLOR_PICKER_FIELD_NUMBER_OF_COLORS } from '@/components/ColorPickerControl/constants';
import { CommonProjectAccessTab } from '@/components/CommonProjectAccessTab';
import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';
import { PopUpTitle } from '@/components/PopUpTitle';
import { TabPanel } from '@/components/TabPanel';
import { getEditingPopUpTitle } from '@/constants/activityStatus';
import { Departments } from '@/constants/departments';
import { getIsAlreadyUsedMessage } from '@/constants/errors';
import { CANT_ASSIGN_ROLE } from '@/constants/notifications';
import { popUpBreakpoints } from '@/constants/popUp';
import {
	employeeAndInternRoles,
	isCEO,
	isGroupManager,
	isHeadOrGroupManager,
	isIntern,
	isManagerRole,
	isNotChiefRole,
	isRoleWithGroup,
	isSysAdmin,
} from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { ColorPickerField } from '@/fields/ColorPickerField';
import { TextField } from '@/fields/Text';
import { FormikWithBackup } from '@/forms/components/FormikWithBackup';
import { StyledDialogContent, StyledForm } from '@/forms/components/styles';
import type { UserFormValues } from '@/forms/types';
import { checkIsCurrentUserId } from '@/helpers/checkIsCurrentUserId';
import { filterAvailableRoles } from '@/helpers/filterAvailableRoles';
import { getFullName } from '@/helpers/formatName';
import { generateImageDataUrl } from '@/helpers/generateImageDataUrl';
import { isHeadOfLdDepartment } from '@/helpers/isHeadOfLdDepartment';
import { useResize } from '@/hooks/useResize';
import { useGetDepartments } from '@/queries/departments/useGetDepartments';
import { useGetFile } from '@/queries/useGetFile';
import { useUser } from '@/queries/useUser';
import {
	CalendaryCountry,
	ComplexConfigurableConfigurationItem,
	RoleType,
} from '@/types/configuration';
import { DepartmentGroup } from '@/types/department';
import { ImageFileSize } from '@/types/file';

import {
	DEFAULT_USER_TAB_HEIGHT,
	entityName,
	formName,
	MAIN_COUNTRY_NAME,
	roleRestrictions,
	UserFormTabs,
} from './constants';
import { GroupMembersTab } from './GroupMembersTab';
import { validationSchema, validationSchemaSA } from './schema';
import {
	StyledLeftSideBox,
	StyledRightSideBox,
	ColorPickerFieldWrapper,
	ColumnWrapper,
	TabWrapper,
	UserNameContainer,
	UserName,
} from './styles';
import { UserFormProps } from './types';
import {
	getAvailableGroupsForUser,
	getPopUpStylesConfig,
	getTabWrapperHeight,
	getUserCurrentValues,
	getUserFormTabs,
} from './utils';

export function UserForm({
	initialValues,
	handleClose,
	onSubmit,
	id,
	isEmailDuplicate,
	onEmailChange,
	hasHRModuleClaim,
}: UserFormProps) {
	const {
		configurableValues: { departmentTypes, roleTypes, countries },
		persistentValues: { locationTypes },
		isLoading: loading,
	} = useConfiguration();
	const { role, id: userId, departmentId } = useAuth();
	const theme = useTheme();
	const isUserHeadOrGroupManager = isHeadOrGroupManager(role);
	const isUserSysAdmin = isSysAdmin(role);
	const isUserGroupManager = isGroupManager(role);
	const isHeadOfLd = isHeadOfLdDepartment(role, departmentId);
	const isCurrentUser = checkIsCurrentUserId(id, userId);
	const isTabletXSWidth = useHasScreenWidth(popUpBreakpoints.tabletXS);
	const isMobileLWidth = useHasScreenWidth(popUpBreakpoints.mobileL);
	const isTableXLWidth = useHasScreenWidth(popUpBreakpoints.tabletXL);
	const [windowWidth, windowHeight] = useResize();

	const [activeTab, setActiveTab] = useState(UserFormTabs.USER);
	const [userTabHeight, setUserTabHeight] = useState(DEFAULT_USER_TAB_HEIGHT);
	const [isGroupTabAvailable, setIsGroupTabAvailable] = useState(false);

	const { data: userData, isFetching, dataUpdatedAt } = useUser(id);

	const user = useMemo(() => {
		if (userData) {
			const { id: userIdValue, firstName, lastName } = userData;

			return {
				id: userIdValue as number,
				fullName: getFullName(firstName, lastName),
			};
		}

		return null;
	}, [userData]);

	const formRef = useRef<HTMLFormElement | null>(null);

	const availableRoles = useMemo(() => {
		if (isHeadOfLd) {
			return roleTypes.filter(({ id: roleId }) =>
				employeeAndInternRoles.includes(roleId)
			);
		}

		return filterAvailableRoles(roleTypes, role, roleRestrictions);
	}, [isHeadOfLd, role, roleTypes]);

	const ldDepartment = useMemo(
		() =>
			departmentTypes.find(
				({ id: departmentTypeId }) => departmentTypeId === Departments.LD
			) ?? null,
		[departmentTypes]
	);

	const { data: departmentsInfo, isLoading: isLoadingDepartments } =
		useGetDepartments();

	const { data, isFetching: isFileFetching } = useGetFile({
		fileId: userData?.avatarFileId ?? undefined,
		imageSize: ImageFileSize.Medium,
	});

	const imgSource = data ? generateImageDataUrl(data) : undefined;

	const getUserValues = useMemo(
		() =>
			getUserCurrentValues(
				userData,
				initialValues,
				departmentTypes,
				roleTypes,
				locationTypes,
				countries,
				departmentsInfo
			),
		[
			userData,
			dataUpdatedAt,
			departmentTypes,
			locationTypes,
			roleTypes,
			countries,
			departmentsInfo,
		]
	);

	useLayoutEffect(() => {
		const height = formRef?.current?.clientHeight;

		if (height) {
			setUserTabHeight(height);
		}
	});

	const wasOriginallyIntern = isIntern(userData?.position);

	const handleSubmit = useCallback(
		async (
			values: UserFormValues,
			{ setSubmitting }: FormikHelpers<UserFormValues>
		) => {
			const {
				firstName,
				lastName,
				email,
				department,
				projects,
				position,
				currentLocation,
				location,
				registeredLocation,
				group,
			} = values;

			const countryId =
				countries.find(({ name }) => name === MAIN_COUNTRY_NAME)?.id ?? 0;

			if (position !== null) {
				const isInternPosition = isIntern(position.id);
				const isPromotedFromIntern = wasOriginallyIntern && !isInternPosition;
				const shouldSendEmail = isUserSysAdmin || isPromotedFromIntern;
				const requestUpdate = {
					...values,
					id,
					firstName,
					lastName,
					position,
					currentLocation: currentLocation?.trim() ? currentLocation : null,
					location,
					registeredLocation,
					email: isInternPosition || !shouldSendEmail ? null : email,
					isActive: true,
					department: isNotChiefRole(values?.position?.id) ? department : null,
					projects: isUserSysAdmin ? null : projects,
					countryId,
					groupId: group?.id,
				};
				await onSubmit(requestUpdate);

				setSubmitting(false);
				handleClose();
			}
		},
		[
			userData,
			wasOriginallyIntern,
			isUserSysAdmin,
			countries,
			id,
			onSubmit,
			handleClose,
		]
	);

	const handleChangeTab = useCallback((newActiveTab: number) => {
		setActiveTab(newActiveTab);
	}, []);

	const showTabs = !isUserSysAdmin;

	const popUpStylesConfig = useMemo(
		() =>
			getPopUpStylesConfig({
				theme,
				hasTabs: showTabs,
				activeTab,
				windowHeight,
				windowWidth,
			}),
		[theme, showTabs, activeTab, windowHeight, windowWidth]
	);

	const tabWrapperHeight = useMemo(
		() =>
			`${getTabWrapperHeight({
				theme,
				activeTab,
				windowHeight,
				windowWidth,
				userTabHeight,
			})}px`,
		[theme, activeTab, windowHeight, windowWidth, userTabHeight]
	);

	const formTabs = useMemo(
		() =>
			showTabs
				? ({
						fullWidth: false,
						type: 'withIcon',
						data: getUserFormTabs(isManagerRole(role) && isGroupTabAvailable),
						onChangeTab: handleChangeTab,
						activeTab,
				  } as const)
				: undefined,
		[showTabs, activeTab, handleChangeTab, isGroupTabAvailable]
	);

	return (
		<FormikWithBackup
			enableReinitialize
			initialValues={getUserValues}
			validationSchema={isUserSysAdmin ? validationSchemaSA : validationSchema}
			onSubmit={handleSubmit}
			onClose={handleClose}
			validateOnBlur={false}
			name={formName}
			id={id}
		>
			{({
				isSubmitting,
				values,
				setFieldValue,
				errors,
				touched,
				setFieldTouched,
				validateField,
				submitForm,
				handleBlur,
				handleOutsideClose,
			}) => {
				const handleOnChange = (field: string) => (value: unknown) => {
					setFieldValue(field, value);
					setFieldTouched(field, true);
					validateField(field);
				};

				const handleOnChangeRegisteredLocation =
					(field: string) => (value: unknown) => {
						const registeredLocationValue = value as CalendaryCountry;
						setFieldValue(field, registeredLocationValue?.id);
						setFieldTouched(field, true);
						validateField(field);
					};

				const {
					isActive,
					position,
					location,
					department,
					group,
					color,
					registeredLocationId,
				} = values;

				const registeredLocation = countries.find(
					(item) => item.id === registeredLocationId
				);

				const handleEmailChange = (name: string, value: string) => {
					setFieldValue(name, value);
					onEmailChange();
				};

				const handleOnBlurEmail = async () => {
					handleBlur('email');
				};

				const handleDepartmentChange = async (
					option: ComplexConfigurableConfigurationItem | null
				) => {
					setFieldValue('department', option);
					setFieldTouched('department', true);
					await validateField('department');

					if (isRoleWithGroup(position?.id)) {
						setFieldValue(
							'group',
							option?.fullName === getUserValues.department?.fullName
								? getUserValues.group
								: undefined
						);
					}
				};

				const handlePositionChange = async (option: RoleType | null) => {
					setFieldValue('position', option);
					setFieldTouched('position', true);
					await validateField('position');

					if (isIntern(option?.id)) {
						setFieldValue('department', ldDepartment);
					}

					if (!isRoleWithGroup(option?.id)) {
						setFieldValue('group', undefined);
					}

					await validateField('email');
					await validateField('department');
				};

				const isDeactivated = isActive === false;
				const isInternPosition = isIntern(position?.id);
				const isDisableRussianFields = useMemo(
					() =>
						isDeactivated ||
						isCurrentUser ||
						(!isUserSysAdmin && isCEO(userData?.position)),
					[isDeactivated, isCurrentUser, userData?.position]
				);

				const emailError =
					!isSubmitting && Boolean(errors.email) && touched.email;

				const departmentValue = isInternPosition ? ldDepartment : department;

				const availableGroups: Array<DepartmentGroup> = useMemo(
					() =>
						getAvailableGroupsForUser(
							departmentsInfo?.find(
								({ departmentName }) =>
									departmentName === departmentValue?.fullName
							)?.groups ?? [],
							position?.id,
							getUserValues.group ?? undefined
						).sort(({ id: leftId }, { id: rightId }) => leftId - rightId),
					[departmentValue?.fullName, departmentsInfo, getUserValues, position]
				);

				const formControls = useMemo(
					() =>
						activeTab === 0
							? {
									positiveControl: {
										onClick: submitForm,
										disabled: isDeactivated,
										tooltipText: isDeactivated
											? "You can't edit deactivated user."
											: undefined,
									},
									negativeControl: { onClick: handleClose },
							  }
							: undefined,
					[handleClose, activeTab, isActive, submitForm, isDeactivated]
				);

				useEffect(() => {
					setIsGroupTabAvailable(
						isGroupManager(getUserValues.position?.id) &&
							isGroupManager(position?.id) &&
							Boolean(group) &&
							getUserValues.group?.id === group?.id
					);
				}, [getUserValues, position, group]);

				const isGroupFieldAvailable = useMemo(
					() => isRoleWithGroup(position?.id),
					[position]
				);

				const {
					firstName: userDisplayedFirstName,
					lastName: userDisplayedLastName,
				} = getUserValues;

				const userFullName = getFullName(
					userDisplayedFirstName,
					userDisplayedLastName
				);

				const isLoaderRequired = loading || isFetching || isLoadingDepartments;
				const areNamesDisabled = isDeactivated || !isUserSysAdmin;
				const isPromotedFromIntern = wasOriginallyIntern && !isInternPosition;

				const isPositionDisabled =
					isDeactivated ||
					((isUserHeadOrGroupManager || isUserSysAdmin) &&
						!hasHRModuleClaim &&
						!isHeadOfLd);

				const canEditEmail =
					isUserSysAdmin || (isPromotedFromIntern && !isPositionDisabled);
				const isEmailDisabled = isDeactivated || !canEditEmail;

				const isPositionTooltipOpen =
					isUserHeadOrGroupManager && !hasHRModuleClaim && !isHeadOfLd;

				const isDepartmentDisabled =
					isDeactivated ||
					isInternPosition ||
					((isUserHeadOrGroupManager || isUserSysAdmin) && !hasHRModuleClaim);

				const isDepartmentTooltipOpen =
					isUserHeadOrGroupManager && !hasHRModuleClaim;

				const isGroupDisabled =
					isDeactivated ||
					((isUserSysAdmin || isUserGroupManager) && !hasHRModuleClaim);

				return (
					<PopUp
						stylesConfig={popUpStylesConfig}
						title={
							<PopUpTitle
								headerText={getEditingPopUpTitle(entityName)}
								extraContent={
									<UserNameContainer variant="m-400">
										(
										<Tooltip
											text={userFullName}
											useIfTextCut
											fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
										>
											<UserName>{userFullName}</UserName>
										</Tooltip>
										)
									</UserNameContainer>
								}
							/>
						}
						onClose={handleClose}
						onShadowClick={handleOutsideClose}
						type="form"
						showLoader={Boolean(isLoaderRequired) || isSubmitting}
						controls={formControls}
						tabs={formTabs}
					>
						<TabPanel value={activeTab} index={UserFormTabs.USER}>
							<StyledForm
								ref={formRef}
								style={{
									overflow: 'unset',
								}}
							>
								<StyledDialogContent>
									<ColumnWrapper>
										<StyledLeftSideBox>
											<Avatar
												firstName={userDisplayedFirstName}
												lastName={userDisplayedLastName}
												src={isFileFetching ? undefined : imgSource}
											/>
											<ColorPickerFieldWrapper>
												<ColorPickerField
													value={color}
													name="color"
													disabled={isDeactivated}
													flexible
													spaceBetween={isTableXLWidth || !isTabletXSWidth}
													numberOfColors={
														isMobileLWidth
															? COLOR_PICKER_FIELD_NUMBER_OF_COLORS.mobileL
															: COLOR_PICKER_FIELD_NUMBER_OF_COLORS.mobile
													}
												/>
											</ColorPickerFieldWrapper>
										</StyledLeftSideBox>

										<StyledRightSideBox>
											<TextField
												name="firstName"
												label="First name"
												onChange={setFieldValue}
												disabled={areNamesDisabled}
												isRequired
											/>
											<TextField
												name="lastName"
												label="Last name"
												onChange={setFieldValue}
												disabled={areNamesDisabled}
												isRequired
											/>
											<TextField
												name="firstNameRu"
												label="First name (russian)"
												onChange={setFieldValue}
												disabled={isDisableRussianFields}
											/>
											<TextField
												name="lastNameRu"
												label="Last name (russian)"
												onChange={setFieldValue}
												disabled={isDisableRussianFields}
											/>
											{!isInternPosition && (
												<TextField
													name="email"
													className="email"
													label="Email"
													disabled={isEmailDisabled}
													onChange={handleEmailChange}
													onBlur={handleOnBlurEmail}
													error={isEmailDuplicate || emailError}
													errorText={
														isEmailDuplicate
															? getIsAlreadyUsedMessage('Email', false)
															: errors.email
													}
													isRequired
												/>
											)}
											<Tooltip
												open={isPositionTooltipOpen}
												text={CANT_ASSIGN_ROLE}
											>
												<div>
													<Autoselect
														label="Position"
														value={position}
														valueField="description"
														options={availableRoles}
														onChange={handlePositionChange}
														error={Boolean(errors.position) && touched.position}
														errorText={errors.position}
														disabled={isPositionDisabled}
														isRequired
													/>
												</div>
											</Tooltip>
											{isNotChiefRole(position?.id) && (
												<Tooltip
													open={isDepartmentTooltipOpen}
													text="You can't choose user's department"
												>
													<div>
														<Autoselect
															label="Department"
															value={departmentValue}
															valueField="fullName"
															options={departmentTypes}
															onChange={handleDepartmentChange}
															error={
																!isDepartmentDisabled &&
																Boolean(errors.department) &&
																touched.department
															}
															errorText={
																!isDepartmentDisabled
																	? errors.department
																	: undefined
															}
															disabled={isDepartmentDisabled}
															isRequired
														/>
													</div>
												</Tooltip>
											)}
											{isGroupFieldAvailable && (
												<Autoselect
													label="Group"
													value={group?.name}
													valueField="name"
													onChange={handleOnChange('group')}
													options={availableGroups}
													error={Boolean(errors.group) && touched.group}
													errorText={errors.group}
													disabled={isGroupDisabled}
												/>
											)}
											<TextField
												name="currentLocation"
												label="Current location"
												onChange={setFieldValue}
												disabled={isDeactivated}
											/>
											<Autoselect
												disabled={isDeactivated || isUserSysAdmin}
												label="Location"
												value={location}
												valueField="description"
												options={locationTypes}
												onChange={handleOnChange('location')}
												error={Boolean(errors.location) && touched.location}
												errorText={errors.location}
												isRequired
											/>
											<Autoselect
												disabled={
													isDeactivated || (isUserSysAdmin && !hasHRModuleClaim)
												}
												label="Registered location"
												value={registeredLocation}
												valueField="name"
												options={countries}
												onChange={handleOnChangeRegisteredLocation(
													'registeredLocationId'
												)}
												error={
													Boolean(errors.registeredLocationId) &&
													touched.registeredLocation
												}
												errorText={errors.registeredLocationId}
												isRequired
											/>
										</StyledRightSideBox>
									</ColumnWrapper>
								</StyledDialogContent>
							</StyledForm>
						</TabPanel>
						{group && department ? (
							<TabPanel value={activeTab} index={UserFormTabs.GROUP_MEMBERS}>
								<TabWrapper height={tabWrapperHeight}>
									<GroupMembersTab
										groupId={group.id}
										departmentId={department.id}
									/>
								</TabWrapper>
							</TabPanel>
						) : null}
						<TabPanel value={activeTab} index={UserFormTabs.PROJECT_ACCESS}>
							<TabWrapper height={tabWrapperHeight}>
								{user && <CommonProjectAccessTab user={user} isUserTab />}
							</TabWrapper>
						</TabPanel>
					</PopUp>
				);
			}}
		</FormikWithBackup>
	);
}
