import { Autoselect } from '@mms/mms-ui-library/components/Autoselects';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { useEffect, useMemo } from 'react';

import {
	DEPARTMENT_DISABLED_TOOLTIP,
	formSections,
	roleRestrictions,
} from '../../constants';
import { FormColumn } from '../../styles';

import { FormSection } from '@/components/FormSection';
import { Departments } from '@/constants/departments';
import { CANT_ASSIGN_ROLE } from '@/constants/notifications';
import {
	isChiefRole,
	isGroupManager,
	isHeadOfDepartment,
	isIntern,
	isRoleWithGroup,
	Roles,
} from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { TextField } from '@/fields/Text';
import { ADMIN_PANEL_USER_FORM_FIELDS } from '@/forms/AdminPanelForms/UserAddingForm/constants';
import { useUserForm } from '@/forms/AdminPanelForms/UserAddingForm/hooks/useUserForm';
import { getRoleRestrictions } from '@/forms/AdminPanelForms/UserAddingForm/utils/getRoleRestrictions';
import { filterAvailableRoles } from '@/helpers/filterAvailableRoles';
import { isHeadOfLdDepartment } from '@/helpers/isHeadOfLdDepartment';
import { useGetDepartments } from '@/queries/departments/useGetDepartments';
import {
	ComplexConfigurableConfigurationItem,
	RoleType,
} from '@/types/configuration';
import { DepartmentGroup } from '@/types/department';

import { WorkDetailsSectionProps } from './types';

export function WorkDetailsSection({
	hasHRModuleClaim,
}: WorkDetailsSectionProps) {
	const {
		values,
		errors,
		touched,
		handleFieldChange,
		setFieldValue,
		setFieldTouched,
		validateField,
	} = useUserForm();

	const { role, departmentId } = useAuth();

	const {
		configurableValues: { departmentTypes, roleTypes },
	} = useConfiguration();

	const { data: departmentsInfo } = useGetDepartments();

	const ldDepartment = useMemo(
		() => departmentTypes.find(({ id }) => id === Departments.LD) ?? null,
		[departmentTypes]
	);

	const hodDepartment = useMemo(
		() => departmentTypes.find(({ id }) => id === departmentId) ?? null,
		[departmentTypes, departmentId]
	);

	const canAssignIntern =
		hasHRModuleClaim || isHeadOfLdDepartment(role, departmentId);
	const isPositionDisabled = !isChiefRole(role) && !canAssignIntern;

	const isInternPosition = isIntern(values.position?.id);

	const departmentValue = useMemo(() => {
		if (isInternPosition) {
			return ldDepartment;
		}

		return values.department;
	}, [isInternPosition, ldDepartment, values.department]);

	const departmentGroups: DepartmentGroup[] = useMemo(() => {
		const groups =
			departmentsInfo?.find(
				({ departmentName }) => departmentName === departmentValue?.fullName
			)?.groups ?? [];

		return isGroupManager(values.position?.id)
			? groups.filter(({ groupManagerEmail }) => !groupManagerEmail)
			: groups;
	}, [departmentValue?.fullName, departmentsInfo, values.position?.id]);

	const isChiefPosition = isChiefRole(values.position?.id);

	const isGroupFieldVisible = isRoleWithGroup(values.position?.id);

	const isDepartmentDisabled =
		isInternPosition || (!hasHRModuleClaim && isHeadOfDepartment(role));

	const availableRoles = useMemo(
		() =>
			filterAvailableRoles(
				roleTypes,
				role,
				getRoleRestrictions(canAssignIntern, roleRestrictions)
			),
		[canAssignIntern, role, roleTypes]
	);

	useEffect(() => {
		if (!isPositionDisabled) {
			return;
		}

		const employeeRole =
			availableRoles.find(({ id }) => id === Roles.SE) ?? null;

		if (employeeRole && values.position?.id !== Roles.SE) {
			setFieldValue(ADMIN_PANEL_USER_FORM_FIELDS.position.name, employeeRole);
		}
	}, [availableRoles, isPositionDisabled, setFieldValue, values.position?.id]);

	useEffect(() => {
		if (!isInternPosition || !ldDepartment) {
			return;
		}

		if (values.department?.id !== ldDepartment.id) {
			setFieldValue(ADMIN_PANEL_USER_FORM_FIELDS.department.name, ldDepartment);
		}
	}, [isInternPosition, ldDepartment, setFieldValue, values.department?.id]);

	const handlePositionChange = async (option: RoleType | null) => {
		const wasInternPosition = isIntern(values.position?.id);

		await setFieldValue(ADMIN_PANEL_USER_FORM_FIELDS.position.name, option);
		setFieldTouched(ADMIN_PANEL_USER_FORM_FIELDS.position.name, true);

		if (isIntern(option?.id)) {
			await setFieldValue(
				ADMIN_PANEL_USER_FORM_FIELDS.department.name,
				ldDepartment
			);
			await setFieldValue(
				ADMIN_PANEL_USER_FORM_FIELDS.corporateEmail.name,
				null
			);
			setFieldTouched(ADMIN_PANEL_USER_FORM_FIELDS.corporateEmail.name, false);
		} else if (!hasHRModuleClaim && isHeadOfDepartment(role)) {
			await setFieldValue(
				ADMIN_PANEL_USER_FORM_FIELDS.department.name,
				hodDepartment
			);
		} else if (wasInternPosition) {
			await setFieldValue(ADMIN_PANEL_USER_FORM_FIELDS.department.name, null);
			setFieldTouched(ADMIN_PANEL_USER_FORM_FIELDS.department.name, false);
		}

		await setFieldValue(ADMIN_PANEL_USER_FORM_FIELDS.group.name, null);

		await Promise.all([
			validateField(ADMIN_PANEL_USER_FORM_FIELDS.position.name),
			validateField(ADMIN_PANEL_USER_FORM_FIELDS.personalEmail.name),
			validateField(ADMIN_PANEL_USER_FORM_FIELDS.corporateEmail.name),
			validateField(ADMIN_PANEL_USER_FORM_FIELDS.contractStartDate.name),
			validateField(ADMIN_PANEL_USER_FORM_FIELDS.contractEndDate.name),
		]);
	};

	const handleDepartmentChange = async (
		option: ComplexConfigurableConfigurationItem | null
	) => {
		await setFieldValue(ADMIN_PANEL_USER_FORM_FIELDS.department.name, option);
		setFieldTouched(ADMIN_PANEL_USER_FORM_FIELDS.department.name, true);
		validateField(ADMIN_PANEL_USER_FORM_FIELDS.department.name);

		setFieldTouched(ADMIN_PANEL_USER_FORM_FIELDS.group.name, false);
		setFieldValue(ADMIN_PANEL_USER_FORM_FIELDS.group.name, null);
	};

	const departmentAutoselect = (
		<Autoselect
			{...ADMIN_PANEL_USER_FORM_FIELDS.department}
			value={departmentValue}
			valueField="fullName"
			options={departmentTypes}
			onChange={handleDepartmentChange}
			disabled={isDepartmentDisabled}
			error={
				!isDepartmentDisabled &&
				Boolean(errors.department) &&
				touched.department
			}
			errorText={!isDepartmentDisabled ? errors.department : undefined}
			isRequired
		/>
	);

	const positionAutoselect = (
		<Autoselect
			{...ADMIN_PANEL_USER_FORM_FIELDS.position}
			value={values.position}
			valueField="description"
			options={availableRoles}
			onChange={handlePositionChange}
			disabled={isPositionDisabled}
			error={Boolean(errors.position) && touched.position}
			errorText={errors.position}
			isRequired
		/>
	);

	return (
		<FormSection title={formSections.workDetails}>
			<FormColumn>
				{isPositionDisabled ? (
					<Tooltip text={CANT_ASSIGN_ROLE}>
						<div>{positionAutoselect}</div>
					</Tooltip>
				) : (
					positionAutoselect
				)}

				{!isChiefPosition &&
					(isDepartmentDisabled ? (
						<Tooltip text={DEPARTMENT_DISABLED_TOOLTIP}>
							<div>{departmentAutoselect}</div>
						</Tooltip>
					) : (
						departmentAutoselect
					))}

				{isGroupFieldVisible && (
					<Autoselect
						{...ADMIN_PANEL_USER_FORM_FIELDS.group}
						value={values.group?.name}
						valueField="name"
						options={departmentGroups}
						onChange={handleFieldChange(
							ADMIN_PANEL_USER_FORM_FIELDS.group.name
						)}
						error={Boolean(errors.group) && touched.group}
						errorText={errors.group}
					/>
				)}

				{!isInternPosition && (
					<TextField
						{...ADMIN_PANEL_USER_FORM_FIELDS.corporateEmail}
						onChange={setFieldValue}
						isRequired
					/>
				)}
			</FormColumn>
		</FormSection>
	);
}
