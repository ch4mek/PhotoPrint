import { MouseEvent, useEffect, useMemo, useState } from 'react';

import { Departments, techDepartments } from '@/constants/departments';
import { isChiefRole } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { usePayloadContext } from '@/context/PayloadContext';

import { INCLUDED_DEPARTMENTS, QUICK_FILTERS_TEXT } from './constants';
import {
	DepartmentsRow,
	DepartmentsFilterButton,
	DepartmentsScroller,
	QuickFiltersLabel,
	QuickFiltersWrapper,
} from './styles';

export function PayloadQuickFilters() {
	const { role } = useAuth();
	const isChief = isChiefRole(role);

	const [selectedDepartment, setSelectedDepartment] = useState('');

	const {
		configurableValues: { departmentTypes },
	} = useConfiguration();

	const {
		gridFilters: { departmentFilter },
		setters: { setDepartmentFilter },
	} = usePayloadContext();

	const filteredDepartments = useMemo(
		() =>
			departmentTypes.filter(({ id }) =>
				INCLUDED_DEPARTMENTS.includes(id as Departments)
			),
		[departmentTypes]
	);

	const handleFilterClick = (e: MouseEvent<HTMLButtonElement>) => {
		const id = e.currentTarget.id;

		const isAlreadySelected = selectedDepartment === id;

		setSelectedDepartment(!isAlreadySelected ? id : '');
		setDepartmentFilter(!isAlreadySelected ? [id] : techDepartments);
	};

	useEffect(() => {
		const isFilterOutOfSync =
			selectedDepartment &&
			(!departmentFilter.includes(selectedDepartment) ||
				departmentFilter.length > 1);

		if (isFilterOutOfSync) {
			setSelectedDepartment('');
		}
	}, [selectedDepartment, departmentFilter]);

	if (!isChief) {
		return null;
	}

	return (
		<QuickFiltersWrapper>
			<QuickFiltersLabel variant="m-400">
				{QUICK_FILTERS_TEXT}:
			</QuickFiltersLabel>
			<DepartmentsScroller>
				<DepartmentsRow>
					{filteredDepartments.map(({ id, fullName }) => (
						<DepartmentsFilterButton
							key={id}
							id={id}
							onClick={handleFilterClick}
							isSelected={selectedDepartment === id}
						>
							{fullName}
						</DepartmentsFilterButton>
					))}
				</DepartmentsRow>
			</DepartmentsScroller>
		</QuickFiltersWrapper>
	);
}
