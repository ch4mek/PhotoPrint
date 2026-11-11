import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { useMemo } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { MerchStorageGridRow } from '../../types';
import { getCellColorForRow } from '../../utils/getCellColorForRow';
import { NameTypography } from '../ItemCellWithPhoto/styles';

import { FIRST_ROWS_HEIGHT } from '@/pages/Merch/components/ElementsTab/constants';
import {
	StorageCellWrapper,
	StorageFieldsWrapper,
	StorageGeneralFields,
	StorageGeneralFieldWrapper,
	StorageCellContentWrapper,
	StorageSubFields,
} from '@/pages/Merch/components/ElementsTab/renderers/ItemStorageCell/styles';

export function StatusCell(props: RenderCellProps<MerchStorageGridRow>) {
	const { row } = props;
	const {
		expanded,
		isClothing,
		storageAvailabilityStatuses,
		status,
		sizeAmounts,
	} = row;
	const theme = useTheme();

	const cellColor = useMemo(
		() => getCellColorForRow(sizeAmounts, theme, expanded),
		[row, theme]
	);

	return (
		<StorageCellWrapper data-expanded={expanded} color={cellColor}>
			<StorageFieldsWrapper>
				<StorageGeneralFields newHeight={FIRST_ROWS_HEIGHT}>
					<StorageGeneralFieldWrapper isFirstRow>
						<StorageCellContentWrapper>
							<Tooltip useIfTextCut text={status}>
								<NameTypography variant="m-400">{status}</NameTypography>
							</Tooltip>
						</StorageCellContentWrapper>
					</StorageGeneralFieldWrapper>
				</StorageGeneralFields>
				{expanded &&
					storageAvailabilityStatuses.map(
						({ storageName, status: statusPerStorage }) => (
							<StorageSubFields key={storageName} isClothing={isClothing}>
								<Tooltip useIfTextCut text={statusPerStorage}>
									<NameTypography variant="s-400">
										{statusPerStorage}
									</NameTypography>
								</Tooltip>
							</StorageSubFields>
						)
					)}
			</StorageFieldsWrapper>
		</StorageCellWrapper>
	);
}
