export default {
    groupBy(array: any[], prop: string | number) {
        return array.reduce(function (groups, item) {
            const val = item[prop];
            groups[val] = groups[val] || {};
            groups[val].key = val;
            groups[val].values = groups[val].values || [];
            groups[val].values.push(item);
            return groups;
        }, {});
    },
    equals(array1: any[], array2: any[]) {
        if (!array1) {
            return false;
        }
        if (!array2) {
            return false;
        }

        if (array1.length !== array2.length) {
            return false;
        }

        for (let i = 0; i < array1.length; i++) {
            if (array1[i] instanceof Array && array2[i] instanceof Array) {
                if (!this.equals(array1[i], array2[i])) {
                    return false;
                }
            }
            else if (array1[i] !== array2[i]) {
                // Warning - two different object instances will never be equal: {x:20} != {x:20}
                return false;
            }
        }
        return true;
    },
    distinct(array: any[], prop: (item: any) => any) {
        if (!Array.isArray(array)) {
            throw new Error("Param 'array' should be an array.");
        }
        if (typeof prop !== "function") {
            throw new Error("Param 'prop' should be a function.");
        }
        const result = [];
        const map = new Map();

        for (const item of array) {
            const propValue = prop(item);
            if (!map.has(propValue)) {
                map.set(propValue, true);
                result.push(item);
            }
        }

        return result;
    }
}