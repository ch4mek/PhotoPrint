import { RenderCellProps } from 'react-data-grid';

import { FIRST_ROWS_HEIGHT } from '../../constants';
import { MerchElementsGridRow } from '../../types';

import {
	StorageCellContentWrapper,
	StorageCellWrapper,
	StorageFieldsWrapper,
	StorageGeneralFields,
	StorageGeneralFieldWrapper,
	StorageSubFields,
} from './styles';

export const itemStorageCell = (
	props: RenderCellProps<MerchElementsGridRow>
) => {
	const { row } = props;
	const { expanded, storage } = row;

	return (
		<StorageCellWrapper data-expanded={expanded}>
			<StorageFieldsWrapper>
				<StorageGeneralFields newHeight={FIRST_ROWS_HEIGHT}>
					<StorageGeneralFieldWrapper isFirstRow>
						<StorageCellContentWrapper>
							{storage.map((item) => item).join(', ')}
						</StorageCellContentWrapper>
					</StorageGeneralFieldWrapper>
				</StorageGeneralFields>
				{expanded &&
					storage.map((item) => (
						<StorageSubFields key={item}>{item}</StorageSubFields>
					))}
			</StorageFieldsWrapper>
		</StorageCellWrapper>
	);
};
