import { Checkbox } from '@mms/mms-ui-library/components/Checkbox';
import {
	ListRowRenderer,
	AutoSizer,
	CellMeasurer,
	CellMeasurerCache,
} from 'react-virtualized';

import { NoDataFoundWrapper } from '@/components/NoDataFoundWrapper';
import { NO_RESULTS_FOUND } from '@/constants/errors';

import {
	ListWrapper,
	ListItem,
	ListContent,
	VirtualizedListWrapper,
} from './styles';
import { ListProps } from './types';

export function List({
	items,
	handleCheckboxClick,
	checkIsChecked,
}: ListProps) {
	const cache = new CellMeasurerCache({
		fixedWidth: true,
	});

	const renderRow: ListRowRenderer = ({ index, key, style, parent }) => {
		const item = items[index];
		const isChecked = checkIsChecked(item.id);

		return (
			<CellMeasurer
				key={key}
				cache={cache}
				parent={parent}
				columnIndex={0}
				rowIndex={index}
			>
				{({ registerChild }) => (
					<ListItem ref={registerChild} style={style}>
						<Checkbox
							label={item.title}
							onClick={handleCheckboxClick(item)}
							checked={isChecked}
							textCut
						/>
					</ListItem>
				)}
			</CellMeasurer>
		);
	};

	return (
		<ListWrapper>
			<ListContent>
				<NoDataFoundWrapper
					title={NO_RESULTS_FOUND}
					isEmptyData={items.length === 0}
				>
					<AutoSizer>
						{({ height, width }) => (
							<VirtualizedListWrapper
								width={width}
								height={height}
								deferredMeasurementCache={cache}
								rowHeight={cache.rowHeight}
								rowRenderer={renderRow}
								rowCount={items.length}
							/>
						)}
					</AutoSizer>
				</NoDataFoundWrapper>
			</ListContent>
		</ListWrapper>
	);
}
