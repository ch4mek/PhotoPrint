import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { DEFAULT_GRID_ROW_HEIGHT } from '@mms/mms-ui-library/constants/grid';
import { memo, useMemo } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { useSpecializationTableContext } from '../../context/specializationTableContext';
import { useSpecializationHardSkillsTableColumns } from '../../hooks/useSpecializationHardSkillsTableColumns';
import { useSpecializationHardSkillsTableRows } from '../../hooks/useSpecializationHardSkillsTableRows';

import { DataGridHoverRowWrapper } from '@/components/Grid/styles';
import { SpecializationRow } from '@/pages/TeamBoardUser/types';

import { overrideHardSkillsStyles } from './styles';

function HardSkillsTableCellInner({
	row,
}: RenderCellProps<SpecializationRow, unknown>) {
	const { snapshots, specializationId, hardSkills, isEven } = row;
	const { allDates } = useSpecializationTableContext();

	const columns = useSpecializationHardSkillsTableColumns(snapshots, allDates);
	const rows = useSpecializationHardSkillsTableRows(
		snapshots,
		hardSkills,
		specializationId
	);

	const overrideHardSkillsGridStyles = useMemo(
		() => overrideHardSkillsStyles(isEven),
		[isEven]
	);

	return (
		<DataGridHoverRowWrapper defineHeight={false}>
			<DataGrid
				columns={columns}
				rows={rows}
				headerRowHeight={0}
				rowHeight={DEFAULT_GRID_ROW_HEIGHT}
				overrideDataGridStyles={overrideHardSkillsGridStyles}
			/>
		</DataGridHoverRowWrapper>
	);
}

export const HardSkillsTableCell = memo(HardSkillsTableCellInner);
