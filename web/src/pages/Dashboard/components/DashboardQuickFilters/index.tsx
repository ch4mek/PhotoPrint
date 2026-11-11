import { useCallback, useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import { useDashboardFilters } from '../../context/DashboardFiltersContext';
import { IncludedDepartments } from '../../types';

import { isChiefRole } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';

import {
	DepartmentsFilterButton,
	DepartmentsRow,
	DepartmentsScroller,
	QuickFiltersLabel,
	QuickFiltersWrapper,
} from './styles';
import { isIncludedDepartment } from './utils/isIncludedDepartment';

export function DashboardQuickFilters() {
	const { selectedDepartmentId, setSelectedDepartmentId } =
		useDashboardFilters();
	const { t } = useTranslation('dashboard');
	const { role } = useAuth();
	const isChief = isChiefRole(role);

	const {
		configurableValues: { departmentTypes },
	} = useConfiguration();

	const filteredDepartments = useMemo(
		() =>
			departmentTypes.filter((department) => isIncludedDepartment(department)),
		[departmentTypes]
	);

	const handleFilterClick = useCallback(
		(departmentId: IncludedDepartments) => () => {
			setSelectedDepartmentId(
				departmentId === selectedDepartmentId ? null : departmentId
			);
		},
		[selectedDepartmentId]
	);

	if (!isChief) {
		return null;
	}

	return (
		<QuickFiltersWrapper>
			<QuickFiltersLabel variant="m-400">
				{t('quickFilters')}:
			</QuickFiltersLabel>
			<DepartmentsScroller>
				<DepartmentsRow>
					{filteredDepartments.map(({ id, fullName }) => (
						<DepartmentsFilterButton
							key={id}
							onClick={handleFilterClick(id)}
							isSelected={id === selectedDepartmentId}
						>
							{fullName}
						</DepartmentsFilterButton>
					))}
				</DepartmentsRow>
			</DepartmentsScroller>
		</QuickFiltersWrapper>
	);
}
