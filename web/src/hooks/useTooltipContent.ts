import { useEffect, useState } from 'react';

import { checkIsTextCut } from '@/helpers/checkIsTextCut';
import { TooltipContent } from '@/types/hooks';

export const useTooltipContent = () => {
	const [tooltipContent, setTooltipContent] = useState<null | TooltipContent>(
		null
	);
	const isTextCut = tooltipContent
		? checkIsTextCut(tooltipContent.anchorElement, 0)
		: false;

	useEffect(() => {
		if (!tooltipContent) {
			return;
		}

		const handleDocumentTouchStart = (event: TouchEvent) => {
			if (!tooltipContent.anchorElement?.contains(event.target as Node)) {
				setTooltipContent(null);
			}
		};

		document.addEventListener('touchstart', handleDocumentTouchStart);

		return () => {
			document.removeEventListener('touchstart', handleDocumentTouchStart);
		};
	}, [tooltipContent]);

	return { tooltipContent, setTooltipContent, isTextCut };
};
