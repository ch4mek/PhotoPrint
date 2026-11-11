import { PickerButton } from '@mms/mms-ui-library/components/PickerButton';
import { OptionDotsIcon } from '@mms/mms-ui-library/icons/base/OptionDotsIcon';
import { useMemo, useRef } from 'react';
import { RenderCellProps } from 'react-data-grid';

import { GridCellDefaultProps } from '@/components/Grid';
import { useAddScrollListener } from '@/hooks/useAddScrollListener';

import { MenuOptionsCellRendererOptions } from './types';

export function menuOptionsCellRenderer<T extends GridCellDefaultProps>(
	{ row }: RenderCellProps<T>,
	optionsConfig?: MenuOptionsCellRendererOptions<T>
) {
	const { options, isDisabled = false } = optionsConfig || {};

	const pickerButtonRef = useRef<HTMLButtonElement>(null);
	const [isOpenPopover, setIsOpenPopover] =
		useAddScrollListener(pickerButtonRef);

	const resolvedOptions = useMemo(() => {
		if (!options) {
			return [];
		}

		return typeof options === 'function' ? options(row) : options;
	}, [options, row]);

	if (!resolvedOptions.length || isDisabled) {
		return null;
	}

	const mappedOptions = useMemo(
		() =>
			resolvedOptions.map((option) => ({
				...option,
				onClick: () => option.onClick?.(row),
			})),
		[resolvedOptions, row]
	);

	return (
		<PickerButton
			icon={<OptionDotsIcon />}
			popoverPosition="left-start"
			options={mappedOptions}
			isOpenPopover={isOpenPopover}
			setIsOpenPopover={setIsOpenPopover}
			ref={pickerButtonRef}
		/>
	);
}
