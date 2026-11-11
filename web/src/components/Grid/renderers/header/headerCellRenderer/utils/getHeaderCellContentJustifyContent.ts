type GetHeaderCellContentJustifyContentParams = {
	isContentAlignedToEnd?: boolean;
	isHasSortableIcon: boolean;
};

export const getHeaderCellContentJustifyContent = ({
	isContentAlignedToEnd,
	isHasSortableIcon,
}: GetHeaderCellContentJustifyContentParams) => {
	if (isContentAlignedToEnd) {
		return 'flex-end';
	}

	if (isHasSortableIcon) {
		return 'space-between';
	}

	return 'flex-start';
};
