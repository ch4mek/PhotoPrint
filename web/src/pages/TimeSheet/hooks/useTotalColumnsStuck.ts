import { DATA_GRID_WRAPPER_CLASS_NAME } from '@mms/mms-ui-library/constants/grid';
import { useEffect } from 'react';

import {
	STUCK_TOTAL_CELL_CLASS_NAME,
	HOVER_TOTAL_CELL_CLASS_NAME,
} from '../components/TimeSheeetManager/constants';

export const useTotalColumnsStuck = (isLoading: boolean) => {
	useEffect(() => {
		const dataGrids = document.getElementsByClassName(
			DATA_GRID_WRAPPER_CLASS_NAME
		);

		if (!isLoading && dataGrids[0]) {
			let totalCells: NodeListOf<Element>;

			const intersectionObserver = new IntersectionObserver(
				([{ intersectionRatio }]) => {
					if (intersectionRatio && intersectionRatio < 1) {
						totalCells.forEach((element) =>
							element.classList.add(STUCK_TOTAL_CELL_CLASS_NAME)
						);
					} else {
						totalCells.forEach((element) =>
							element.classList.remove(STUCK_TOTAL_CELL_CLASS_NAME)
						);
					}
				},
				{
					threshold: [1],
					root: null,
					rootMargin: '0px',
				}
			);

			const mutationObserver = new MutationObserver(() => {
				intersectionObserver.disconnect();

				totalCells = document.querySelectorAll(
					`.${HOVER_TOTAL_CELL_CLASS_NAME}`
				);
				totalCells.forEach((element) =>
					element.classList.remove(STUCK_TOTAL_CELL_CLASS_NAME)
				);

				intersectionObserver.observe(totalCells[0]);
			});

			mutationObserver.observe(dataGrids[0], {
				childList: true,
				subtree: true,
			});

			return () => {
				intersectionObserver.disconnect();
				mutationObserver.disconnect();
			};
		}
	}, [isLoading]);
};
