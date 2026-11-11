import {
	PickerButton,
	PickerButtonOption,
} from '@mms/mms-ui-library/components/PickerButton';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { OptionDotsIcon } from '@mms/mms-ui-library/icons/base/OptionDotsIcon';
import { useCallback, useMemo } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { DepartmentGridRowType } from '../../../../types';
import {
	DEPARTMENT_PICKER_BUTTON_CLASS,
	GROUP_SUBROW_CLASS_NAME,
	HEIGHT_CELL,
} from '../../constants';
import { CollapsibleCellWithMenuProps } from '../../types';
import { newCellHeight } from '../../utils/getNewCellHeight';

import { ChevronButton } from '@/components/ChevronButton';
import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';
import {
	isGroupManager,
	isHeadOrGroupManager,
	isSysAdmin,
} from '@/constants/roles';
import { zIndexes } from '@/constants/zIndexes';
import { useAuth } from '@/context/AuthContext';
import { Group } from '@/types/department';

import {
	CollapsibleCellWithMenuTypography,
	DepartmentsCellWrapper,
	DepartmentsFieldsWrapper,
	DepartmentsGeneralFields,
	DepartmentsGeneralFieldWrapper,
	DepartmentsSubFields,
} from './styles';

export function DepartmentCellRenderer<T extends CollapsibleCellWithMenuProps>({
	row,
	onRowChange,
	menuOptions: { groupRowMenuOptions, departmentRowMenuOptions },
}: RenderCellProps<T> & {
	menuOptions: {
		groupRowMenuOptions: PickerButtonOption[];
		departmentRowMenuOptions: PickerButtonOption[];
	};
}) {
	const { role, departmentId: userDepartmentId } = useAuth();
	const { isExpanded, groups, id, name, type, departmentId } = row;

	const updateRow = useCallback(
		(newRowParts: CollapsibleCellWithMenuProps) => {
			const height = newRowParts.isExpanded!
				? newCellHeight(newRowParts.isExpanded!, groups.length)
				: HEIGHT_CELL;

			onRowChange({
				...row,
				...newRowParts,
				departmetItemsRowHeight: height,
			});
		},
		[onRowChange, row, groups.length]
	);

	const handleExpandedCell = useCallback(() => {
		updateRow({ isExpanded: !isExpanded });
	}, [isExpanded, updateRow]);
	const hasGroups = groups.length > 0;

	const isUserHaveAcessToSeeMenuAndSubrows = useMemo(() => {
		if (isHeadOrGroupManager(role)) {
			return userDepartmentId === departmentId;
		}

		return true;
	}, [departmentId, userDepartmentId]);

	const isUserAllowedToModifyGroups =
		!isGroupManager(role) && !isSysAdmin(role);

	const firstRowContent = (
		<Tooltip
			zIndex={zIndexes.gridTooltip}
			text={name}
			position="bottom"
			fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
			useIfTextCut
		>
			<CollapsibleCellWithMenuTypography
				isSubRow={type === DepartmentGridRowType.Group}
				variant={type === DepartmentGridRowType.Group ? 's-400' : 'm-400'}
			>
				{name}
			</CollapsibleCellWithMenuTypography>
		</Tooltip>
	);

	return (
		<DepartmentsCellWrapper data-expanded={isExpanded}>
			<DepartmentsFieldsWrapper>
				<DepartmentsGeneralFields
					className={GROUP_SUBROW_CLASS_NAME}
					data-subrowhoverindex={id}
					newHeight={HEIGHT_CELL}
				>
					<DepartmentsGeneralFieldWrapper>
						{firstRowContent}
						{hasGroups && (
							<ChevronButton
								tabIndex={id as number}
								onClick={handleExpandedCell}
								isActive={isExpanded}
							/>
						)}
						{isUserHaveAcessToSeeMenuAndSubrows &&
							isUserAllowedToModifyGroups &&
							(groupRowMenuOptions.length ||
								departmentRowMenuOptions.length) && (
								<PickerButton
									className={DEPARTMENT_PICKER_BUTTON_CLASS}
									icon={<OptionDotsIcon />}
									options={departmentRowMenuOptions}
									item={row}
									popoverPosition="left-start"
								/>
							)}
					</DepartmentsGeneralFieldWrapper>
				</DepartmentsGeneralFields>
				{groups.map((group: Group & { type: DepartmentGridRowType }) => (
					<DepartmentsSubFields
						key={group.id}
						className={GROUP_SUBROW_CLASS_NAME}
						data-subrowhoverindex={`${id}-${group.id}`}
					>
						<Tooltip
							zIndex={zIndexes.gridTooltip}
							text={group.name}
							position="bottom"
							fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
							useIfTextCut
						>
							<CollapsibleCellWithMenuTypography
								isSubRow={group.type === DepartmentGridRowType.Group}
								variant="s-400"
							>
								{group.name}
							</CollapsibleCellWithMenuTypography>
						</Tooltip>
						{isUserHaveAcessToSeeMenuAndSubrows &&
							isUserAllowedToModifyGroups &&
							(groupRowMenuOptions.length ||
								departmentRowMenuOptions.length) && (
								<PickerButton
									className={DEPARTMENT_PICKER_BUTTON_CLASS}
									icon={<OptionDotsIcon />}
									options={groupRowMenuOptions}
									item={group}
									popoverPosition="left-start"
								/>
							)}
					</DepartmentsSubFields>
				))}
			</DepartmentsFieldsWrapper>
		</DepartmentsCellWrapper>
	);
}
