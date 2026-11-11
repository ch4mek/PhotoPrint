import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { useMemo } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { MinAmountTypography } from '../../../StorageTab/renderers/MinAmountCell/styles';
import { getCellColorByAmount } from '../../../StorageTab/utils/getCellColorByAmount';
import { getCellColorForRow } from '../../../StorageTab/utils/getCellColorForRow';
import { MerchKitsGridRow } from '../../types';

import { FIRST_ROWS_HEIGHT } from '@/pages/Merch/components/ElementsTab/constants';
import {
	SizeCellContentWrapper,
	SizeCellWrapper,
	SizeFieldsWrapper,
	SizeGeneralFields,
	SizeGeneralFieldWrapper,
	SizeSubFields,
} from '@/pages/Merch/components/ElementsTab/renderers/SizeCell/styles';
import { MerchStatuses } from '@/pages/Merch/constants';

export function PossibleKitsCell(props: RenderCellProps<MerchKitsGridRow>) {
	const { row } = props;
	const {
		expanded,
		storage,
		isClothing,
		possibleKits,
		totalPossibleKits,
		status: rowStatus,
	} = row;
	const theme = useTheme();

	const isChevronShown =
		(isClothing || possibleKits.length > 1) && Boolean(possibleKits.length);

	const cellColor = useMemo(
		() => getCellColorForRow(possibleKits, theme, expanded),
		[row, theme]
	);

	const isDisabled = rowStatus === MerchStatuses.archived;

	return (
		<SizeCellWrapper
			disabled={isDisabled}
			data-expanded={expanded}
			color={cellColor}
		>
			<SizeFieldsWrapper>
				<SizeGeneralFields newHeight={FIRST_ROWS_HEIGHT}>
					<SizeGeneralFieldWrapper isFirstRow>
						<SizeCellContentWrapper>
							<Tooltip useIfTextCut text={totalPossibleKits}>
								<MinAmountTypography variant="m-400">
									{totalPossibleKits}
								</MinAmountTypography>
							</Tooltip>
						</SizeCellContentWrapper>
					</SizeGeneralFieldWrapper>
				</SizeGeneralFields>
				{isChevronShown &&
					expanded &&
					storage.map((item) => {
						const filteredOdpItems = possibleKits.filter(
							({ storageName }) => storageName === item
						);

						return filteredOdpItems.map(
							({ sizeId, possibleKitAmount, status }) => (
								<SizeSubFields
									key={`${item}.${sizeId}`}
									color={getCellColorByAmount(theme, possibleKitAmount, status)}
								>
									{possibleKitAmount}
								</SizeSubFields>
							)
						);
					})}
			</SizeFieldsWrapper>
		</SizeCellWrapper>
	);
}
