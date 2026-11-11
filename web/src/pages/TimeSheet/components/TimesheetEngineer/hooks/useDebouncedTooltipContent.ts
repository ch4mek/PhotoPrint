import debounce from 'lodash/debounce';
import { useMemo } from 'react';

import { TOOLTIP_DELAY } from '../constants';

import { useTooltipContent } from '@/hooks/useTooltipContent';

export const useDebouncedTooltipContent = () => {
	const { tooltipContent, setTooltipContent } = useTooltipContent();

	const debouncedSetTooltipContent = useMemo(
		() => debounce(setTooltipContent, TOOLTIP_DELAY),
		[]
	);

	return { tooltipContent, setTooltipContent: debouncedSetTooltipContent };
};
