import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { RenderCellProps } from 'react-data-grid';

import { PhotoItem } from '../../components/NameWithPhotoItem/NameWithPhotoItem';
import { PopupWithFullImage } from '../../components/PopupWithFullImage/PopupWithFullImage';
import { FIRST_ROWS_HEIGHT, getGridPhotoStyleConfig } from '../../constants';
import { MerchElementsGridRow } from '../../types';
import { getNewCellHeight } from '../../utils/getNewCellHeight';

import { ChevronButton } from '@/components/ChevronButton';
import { useConfiguration } from '@/context/Configuration';
import { useShowModal } from '@/pages/hooks/useShowModal';

import {
	ItemCellWithPhotoContentWrapper,
	NameCellWrapper,
	NameFieldsWrapper,
	NameGeneralFields,
	NameGeneralFieldWrapper,
	NameTypography,
} from './styles';

export function ItemCellWithPhoto({
	row,
	onRowChange,
}: RenderCellProps<MerchElementsGridRow>) {
	const { handleClose, handleOpen, isOpen } = useShowModal();
	const theme = useTheme();
	const {
		configurableValues: { merchSizes },
	} = useConfiguration();
	const { expanded, name, photoFileId, isClothing, description, storage } = row;

	const handleOpenFullPhotoPopup = () => {
		handleOpen();
	};

	const handleExpandedCell = () => {
		onRowChange({
			...row,
			expanded: !expanded,
			itemsRowHeight: !expanded
				? getNewCellHeight(storage, merchSizes, isClothing)
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
								imageId={photoFileId}
								styleConfig={photoItemStyleConfig}
								isResized
							/>
							<Tooltip useIfTextCut text={name}>
								<NameTypography variant="m-400" Component="p">
									{name}
								</NameTypography>
							</Tooltip>
						</ItemCellWithPhotoContentWrapper>
						{isClothing && (
							<ChevronButton onClick={handleExpandedCell} isActive={expanded} />
						)}
					</NameGeneralFieldWrapper>
				</NameGeneralFields>
			</NameFieldsWrapper>
			{isOpen && (
				<PopupWithFullImage
					handleClose={handleClose}
					description={description}
					name={name}
					photoFileId={photoFileId}
				/>
			)}
		</NameCellWrapper>
	);
}
