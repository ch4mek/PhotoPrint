import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { RenderCellProps } from 'react-data-grid';

import { PhotoItem } from '../../../ElementsTab/components/NameWithPhotoItem/NameWithPhotoItem';
import { PopupWithFullImage } from '../../../ElementsTab/components/PopupWithFullImage/PopupWithFullImage';
import {
	FIRST_ROWS_HEIGHT,
	getGridPhotoStyleConfig,
} from '../../../ElementsTab/constants';
import {
	ItemCellWithPhotoContentWrapper,
	NameCellWrapper,
	NameFieldsWrapper,
	NameGeneralFields,
	NameGeneralFieldWrapper,
	NameTypography,
} from '../../../ElementsTab/renderers/ItemCellWithPhoto/styles';
import { MerchTransactionsGridRow } from '../../types';
import { getNewCellHeight } from '../../utils/getNewCellHeight';

import { ChevronButton } from '@/components/ChevronButton';
import { useShowModal } from '@/pages/hooks/useShowModal';

export function ItemCellWithPhoto({
	row,
	onRowChange,
}: RenderCellProps<MerchTransactionsGridRow>) {
	const { handleClose, handleOpen, isOpen } = useShowModal();
	const theme = useTheme();
	const {
		expanded,
		elementName,
		elementDescription,
		elementFileId,
		isClothing,
		transactions,
	} = row;

	const handleOpenFullPhotoPopup = () => {
		handleOpen();
	};

	const handleExpandedCell = () => {
		onRowChange({
			...row,
			expanded: !expanded,
			itemsRowHeight: !expanded
				? getNewCellHeight(transactions)
				: FIRST_ROWS_HEIGHT,
		});
	};

	const photoItemStyleConfig = getGridPhotoStyleConfig(theme);

	return (
		<NameCellWrapper data-expanded={expanded}>
			<NameFieldsWrapper>
				<NameGeneralFields>
					<NameGeneralFieldWrapper>
						<ItemCellWithPhotoContentWrapper>
							<PhotoItem
								onClick={handleOpenFullPhotoPopup}
								imageId={elementFileId}
								styleConfig={photoItemStyleConfig}
								isResized
							/>
							<Tooltip useIfTextCut text={elementName}>
								<NameTypography variant="m-400" Component="p">
									{elementName}
								</NameTypography>
							</Tooltip>
						</ItemCellWithPhotoContentWrapper>
						{isClothing && transactions.length > 1 && (
							<ChevronButton onClick={handleExpandedCell} isActive={expanded} />
						)}
					</NameGeneralFieldWrapper>
				</NameGeneralFields>
			</NameFieldsWrapper>
			{isOpen && (
				<PopupWithFullImage
					handleClose={handleClose}
					description={elementDescription}
					name={elementName}
					photoFileId={elementFileId}
				/>
			)}
		</NameCellWrapper>
	);
}
