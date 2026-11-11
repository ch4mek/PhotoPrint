import { IncludedDepartmentConfigurationItem } from '../types';

import { includedDepartments } from '@/constants/dashboard';
import { IncludedDepartments } from '@/pages/Dashboard/types';
import { ComplexConfigurableConfigurationItem } from '@/types/configuration';

export function isIncludedDepartment(
	departmentType: ComplexConfigurableConfigurationItem
): departmentType is IncludedDepartmentConfigurationItem {
	return includedDepartments.includes(departmentType.id as IncludedDepartments);
}
