import { useCallback, useEffect, useState } from 'react';

export const useGridRowsExpansion = (expandableParentIds: Set<string>) => {
	const [isAllRowsOpen, setIsAllRowsOpen] = useState(true);
	const [expandedIds, setExpandedIds] = useState<Set<string>>(new Set());

	useEffect(() => {
		if (isAllRowsOpen) {
			setExpandedIds(expandableParentIds);
		}
	}, [isAllRowsOpen, expandableParentIds]);

	const handleOpenRowsToggleChange = useCallback(
		(checked: boolean) => {
			setIsAllRowsOpen(checked);
			setExpandedIds(checked ? expandableParentIds : new Set());
		},
		[expandableParentIds]
	);

	const handleToggleExpand = useCallback((id: string) => {
		setExpandedIds((prev) => {
			const next = new Set(prev);

			if (next.has(id)) {
				next.delete(id);
			} else {
				next.add(id);
			}

			return next;
		});
	}, []);

	return {
		expandedIds,
		isAllRowsOpen,
		handleOpenRowsToggleChange,
		handleToggleExpand,
	};
};
