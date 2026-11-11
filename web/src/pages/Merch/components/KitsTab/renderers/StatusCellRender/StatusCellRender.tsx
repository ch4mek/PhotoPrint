import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { useMemo } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { getCellColorForRow } from '../../../StorageTab/utils/getCellColorForRow';
import { MerchKitsGridRow } from '../../types';
import { NameTypography } from '../ItemCellWithPhoto/styled';

import { FIRST_ROWS_HEIGHT } from '@/pages/Merch/components/ElementsTab/constants';
import {
	StorageCellWrapper,
	StorageFieldsWrapper,
	StorageGeneralFields,
	StorageGeneralFieldWrapper,
	StorageCellContentWrapper,
	StorageSubFields,
} from '@/pages/Merch/components/ElementsTab/renderers/ItemStorageCell/styles';
import { MerchStatuses } from '@/pages/Merch/constants';

export function StatusCell(props: RenderCellProps<MerchKitsGridRow>) {
	const { row } = props;
	const {
		expanded,
		isClothing,
		storageAvailabilityStatuses,
		status,
		possibleKits,
	} = row;
	const theme = useTheme();

	const cellColor = useMemo(
		() => getCellColorForRow(possibleKits, theme, expanded),
		[row, theme]
	);

	const isDisabled = status === MerchStatuses.archived;

	return (
		<StorageCellWrapper
			disabled={isDisabled}
			data-expanded={expanded}
			color={cellColor}
		>
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
