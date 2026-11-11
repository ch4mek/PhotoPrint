import { useMemo, useEffect, useReducer } from 'react';

import { equipmentReducer, createSetFilters } from '../../utils';

import { PageHeader } from '@/components/PageHeader';
import { INITIAL_ORDER_BY_VALUE } from '@/constants/equipment';
import { NO_VALUE_FILTER_OPTION } from '@/constants/filters';
import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { isHeadOrGroupManager } from '@/constants/roles';
import { SortDirection } from '@/constants/sort';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { EquipmentProvider } from '@/context/EquipmentContext/Equipment';
import { EquipmentForm } from '@/forms/EquipmentForm';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { useInitialFiltersWithDepartmentByDefault } from '@/hooks/useInitialFiltersWithDepartmentByDefault';
import { usePageWithSearch } from '@/hooks/usePageWithSearch';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useTableState } from '@/pages/hooks/useTableState';
import { PageWrapper } from '@/pages/styles';
import { UserClaims } from '@/types/claims';

import { DEFAULT_EQUIPMENT_FILTERS } from './constants';
import { EquipmentExportControl } from './EquipmentExportButton/EquipmentExportButton';
import { EquipmentGrid } from './EquipmentGrid';

export function EquipmentTab() {
	const { role, departmentId, claims } = useAuth();
	const {
		rowsPerPage,
		setRowsPerPage,
		order,
		orderBy,
		sortColumns,
		handleSortColumnChange,
	} = useTableState({
		initialRowsPerPage: Number(DEFAULT_ROWS_PER_PAGE),
		initialOrder: SortDirection.DESC,
		initialOrderBy: INITIAL_ORDER_BY_VALUE,
	});
	const { page, setPage } = usePageWithSearch();

	const initialFilters = useInitialFiltersWithDepartmentByDefault(
		DEFAULT_EQUIPMENT_FILTERS,
		role,
		departmentId,
		claims,
		[UserClaims.ManageAllEquipment]
	);

	const [filters, dispatch] = useReducer(equipmentReducer, initialFilters);
	const setFilters = useMemo(() => createSetFilters(dispatch), []);

	const { id, handleOpen, handleClose, isOpen } = useShowModal();

	useEffect(() => setPage(0), [filters, sortColumns]);

	const {
		configurableValues: { departmentTypes, equipmentTypes },
		persistentValues: { equipmentLocationTypes, equipmentAvailabilityTypes },
	} = useConfiguration();

	const departments = useMemo(
		() => [...departmentTypes, NO_VALUE_FILTER_OPTION],
		[departmentTypes]
	);

	const hasManageAllEquipmentClaim = checkIsHasClaims(
		[UserClaims.ManageAllEquipment],
		claims
	);
	const isAddButtonShown =
		!isHeadOrGroupManager(role) || hasManageAllEquipmentClaim;

	return (
		<>
			<PageWrapper>
				<PageHeader
					addButton={{ onClick: handleOpen, isHidden: !isAddButtonShown }}
					exportComponent={
						<EquipmentExportControl
							filters={filters}
							order={order}
							orderBy={orderBy}
							allDepartments={departments}
							allEquipmentLocations={equipmentLocationTypes}
							allEquipmentTypes={equipmentTypes}
							allAvailabilityTypes={equipmentAvailabilityTypes}
							isAddButtonShown={isAddButtonShown}
						/>
					}
				/>
				<EquipmentGrid
					filters={filters}
					setFilters={setFilters}
					rowsPerPage={rowsPerPage}
					page={page}
					handleOpen={handleOpen}
					sortColumns={sortColumns}
					onSortColumnsChange={handleSortColumnChange}
					setRowsPerPage={setRowsPerPage}
					setPage={setPage}
					equipmentTypes={equipmentTypes}
					equipmentLocationTypes={equipmentLocationTypes}
					departments={departments}
					initialFilters={initialFilters}
					equipmentAvailabilityTypes={equipmentAvailabilityTypes}
					order={order}
					orderBy={orderBy}
				/>
			</PageWrapper>

			{isOpen && (
				<EquipmentProvider>
					<EquipmentForm id={id as string} onClose={handleClose} />
				</EquipmentProvider>
			)}
		</>
	);
}
